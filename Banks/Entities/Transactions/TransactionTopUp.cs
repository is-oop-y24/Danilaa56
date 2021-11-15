﻿using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionTopUp : Transaction
    {
        private decimal _amount;

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

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] += Amount - Commission;
        }

        public override void Reverse(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] -= Amount - Commission;
        }
    }
}