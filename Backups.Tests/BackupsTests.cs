﻿using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Server.Entities;
using NUnit.Framework;

namespace Backups.Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(Path.Combine("tmp", "data"));
            Directory.CreateDirectory(Path.Combine("tmp", "repo"));

            File.WriteAllText(Path.Combine("tmp", "data", "file1.txt"), "Hello 1");
            File.WriteAllText(Path.Combine("tmp", "data", "file2.txt"), "Hello 2 and longer");
        }

        [Test]
        public void TestBackupJob()
        {
            IRepository repo = new RepositoryLocal(Path.Combine("tmp", "repo"));

            var backupJob = new BackupJob(repo);
            backupJob.ObjectDistributor = new SplitStorageDistributor();
            var jobObject1 = new JobObjectLocal(Path.Combine("tmp", "data"), "file1.txt");
            var jobObject2 = new JobObjectLocal(Path.Combine("tmp", "data"), "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            backupJob.CreateRestorePoint();

            backupJob.Remove(jobObject1);
            backupJob.CreateRestorePoint();

            Assert.AreEqual(2, backupJob.GetRestorePoints().Count);
            Assert.AreEqual(3, repo.GetStorages().Length);
        }

        [Test]
        public void TestLocalRepo()
        {
            IRepository repo = new RepositoryLocal(Path.Combine("tmp", "repo"));

            var backupJob = new BackupJob(repo);
            var jobObject1 = new JobObjectLocal(Path.Combine("tmp", "data"), "file1.txt");
            var jobObject2 = new JobObjectLocal(Path.Combine("tmp", "data"), "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            Assert.AreEqual(true, File.Exists(Path.Combine("tmp", "repo", restorePoint1.Storages[0].Id)));
        }

        [Test]
        public void TestRemoteRepo()
        {
            var server = new RepositoryRemoteServer(8080, Path.Combine("tmp", "repo"));
            server.Start();

            IRepository repo = new RepositoryRemote("localhost", 8080);

            var backupJob = new BackupJob(repo);
            var jobObject1 = new JobObjectLocal(Path.Combine("tmp", "data"), "file1.txt");
            var jobObject2 = new JobObjectLocal(Path.Combine("tmp", "data"), "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            Assert.AreEqual(true, File.Exists(Path.Combine("tmp", "repo", restorePoint1.Storages[0].Id)));
        }

        [Test]
        public void TestRemoteGetStorageIds()
        {
            var server = new RepositoryRemoteServer(8080, Path.Combine("tmp", "repo"));
            server.Start();

            IRepository repo = new RepositoryRemote("localhost", 8080);

            var backupJob = new BackupJob(repo);
            var jobObject1 = new JobObjectLocal(Path.Combine("tmp", "data"), "file1.txt");
            var jobObject2 = new JobObjectLocal(Path.Combine("tmp", "data"), "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            var storageIds = restorePoint1.Storages.Select(storage => storage.Id).ToHashSet();

            Assert.That(repo.GetStorages().ToHashSet().SetEquals(storageIds));
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete("tmp", true);
        }
    }
}