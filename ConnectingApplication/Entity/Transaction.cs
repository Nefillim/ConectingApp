using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ConectingApp.ConnectingApplication.Entity
{
    public struct Transaction
    {
        public string TransactionId { get; private set; }
        public float Value { get; private set; }

        public Transaction(string id, float value)
        {
            TransactionId = id;
            Value = value;
        }
    }
}
