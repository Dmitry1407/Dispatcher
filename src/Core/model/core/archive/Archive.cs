using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Core.model.core.archive
{
    public abstract class Archive
    {
        public ArchiveType Type { get; protected set; }

        protected String dbPath;
        protected SQLiteConnection connection;
        protected SQLiteCommand command;
        protected Object locker = new Object();

        private void CreateDB()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            try
            {
                connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;");
                connection.Open(); // Connection Open
                CreateTable();
                connection.Close(); // Connection Close
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("DB [{0}] creating ERROR!", dbPath);
            }
        }

        public void Connect()
        {
            CreateDB();

            try
            {
                connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;");
                connection.Open(); // Connection Open
                command.Connection = connection;
                Console.WriteLine("DB [{0}] CONNECT OPEN", dbPath);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("DB [{0}] connecting ERROR!", dbPath);
            }
        }

        public void Disconnect()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
                Console.WriteLine("DB [{0}] CONNECT CLOSE", dbPath);
            }
        }

        protected abstract void CreateTable();
        public abstract void Insert(Int32 channelID, String value, Int64 timeStamp);
        public abstract List<ArchiveItem> Read(Int32 channelID);
        public abstract List<ArchiveItem> ReadAll();

        public ConnectionState State()
        {
            return connection.State;
        }
    }
}
