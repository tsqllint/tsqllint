﻿using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataCompressionOptionRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "data-compression";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Expected table to use data compression";
            }
        }

        private readonly Action<string, string, int, int> ErrorCallback;

        public DataCompressionOptionRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CreateTableStatement node)
        {
            var childCompressionVisitor = new ChildCompressionVisitor();
            node.AcceptChildren(childCompressionVisitor);

            if (!childCompressionVisitor.CompressionOptionExists)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }

        public class ChildCompressionVisitor : TSqlFragmentVisitor
        {
            public bool CompressionOptionExists
            {
                get; 
                private set;
            }

            public override void Visit(DataCompressionOption node)
            {
                CompressionOptionExists = true;
            }
        }
    }
}