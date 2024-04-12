using RepoDb;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;
public class DBTrace : ITrace
{
    void ITrace.AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
        Console.WriteLine($"After Execution SQL Statement: {log.ExecutionTime} IsError:{log.BeforeExecutionLog.IsThrowException}");
    }    

    void ITrace.BeforeExecution(CancellableTraceLog log)
    {        
        
        Console.WriteLine($"Before Execution SQL Statement: {log.Statement} Key: {log.Key}");
    }

    Task ITrace.AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken)
    {
        Console.WriteLine($"After Execution SQL Statement: {log.ExecutionTime}");
        return null;
    }
    Task ITrace.BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken)
    {
        
        Console.WriteLine($"Async Before Execution SQL Statement: {log.Statement}");
        return null;
    }
}

public static class TraceFactory
{
    private static object _syncLock = new object();
    private static ITrace _trace = null;

    public static ITrace CreateTracer()
    {
        if (_trace == null)
        {
            lock (_syncLock)
            {
                if (_trace == null)
                {
                    _trace = new DBTrace();
                }
            }
        }
        return _trace;
    }
}

