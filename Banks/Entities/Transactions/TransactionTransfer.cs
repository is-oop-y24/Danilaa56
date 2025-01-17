﻿using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionTransfer : Transaction
    {
        private decimal _amount;
        private decimal _receiverAmount;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Amount must be positive", nameof(value));
                _amount = value;
            }
        }

        public decimal Commission { get; set; }
        public Account ReceiverAccount { get; set; }

        public decimal ReceiverAmount
        {
            get => _receiverAmount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Amount must be positive", nameof(value));
                _receiverAmount = value;
            }
        }

        public decimal ReceiverCommission { get; set; }

        public override void Process(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney[Account.Id] -= Amount + Commission;
            accountToMoney[ReceiverAccount.Id] += Amount - Commission;
        }

        public override void Reverse(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney[Account.Id] += Amount + Commission;
            accountToMoney[ReceiverAccount.Id] -= Amount - Commission;
        }
    }
}