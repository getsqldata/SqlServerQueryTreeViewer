﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerParseTreeViewer
{
    internal class OptimizerInfoTracker : InfoTracker
    {
        private static string _beforeTempTableName = "#opt_info_7A70EBC47D934D4E9C0C8645CCE56D10";
        private static string _afterTempTableName = "#opt_info_B34F0C204E08425199739B4F77AEE039";

        private static string _captureBeforeData = "select * into " + _beforeTempTableName + " from sys.dm_exec_query_optimizer_info;";
        private static string _captureAfterData = "select * into " + _afterTempTableName + " from sys.dm_exec_query_optimizer_info;";
        private static string _dropBeforeTable = string.Format("if object_id('tempdb..{0}') is not null drop table {0};", _beforeTempTableName);
        private static string _dropAfterTable = string.Format("if object_id('tempdb..{0}') is not null drop table {0};", _afterTempTableName);
        private static string _dropTables = _dropBeforeTable + _dropAfterTable;

        public static void PreExecute(SqlConnection connection)
        {
            // Run the before and after scripts once just to make sure they are in the plan cache and don't skew the results
            ExecuteNonQuery(connection, _captureBeforeData);
            ExecuteNonQuery(connection, _captureAfterData);

            // Drop the temporary tables
            ExecuteNonQuery(connection, _dropTables);

            // Capture the before data
            ExecuteNonQuery(connection, _captureBeforeData);
        }

        public static void PostExecute(SqlConnection connection)
        {
            // Capture the after data
            ExecuteNonQuery(connection, _captureAfterData);
        }

        public static DataTable AnalyzeResults(SqlConnection connection)
        {
            // Query the differences
            string sql = "select a.counter, a.occurrence - b.occurrence occurrence, a.occurrence * a.value - b.occurrence * b.value value from " +
                _beforeTempTableName + " b join " + _afterTempTableName + " a on a.counter = b.counter where a.occurrence != b.occurrence order by counter;";
            DataTable differenceTable = ExecuteQuery(connection, sql);

            try
            {
                // Drop the temporary tables
                ExecuteNonQuery(connection, _dropTables);

                return differenceTable;
            }
            catch
            {
                if (differenceTable != null)
                {
                    differenceTable.Dispose();
                    differenceTable = null;
                }
                throw;
            }
        }
    }
}
