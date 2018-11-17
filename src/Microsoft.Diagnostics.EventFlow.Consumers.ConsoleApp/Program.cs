// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
using log4net;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace Microsoft.Diagnostics.EventFlow.Consumers.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DiagnosticPipeline pipeline = DiagnosticPipelineFactory.CreatePipeline(".\\eventFlowConfig.json"))
            {
                #region log4net input with stacks and global context
                var logger = LogManager.GetLogger("EventFlowRepo", "MY_LOGGER_NAME");
                GlobalContext.Properties["GlobalContext"] = "My Global Context";

                using (ThreadContext.Stacks["NDC"].Push("Thread Context-1"))
                {
                    using (ThreadContext.Stacks["NDC"].Push("Thread Context-1-1"))
                    {
                        using (LogicalThreadContext.Stacks["LogicalThreadContext"].Push("Logical Thread Context-1-1-1"))
                        {
                            logger.Debug("Hey! Listen!", new Exception("uhoh"));
                        }
                        logger.Info("From Thread Context 1-1");
                    }
                    logger.Info("From Thread Context 1");
                }
                #endregion

                Console.ReadLine();

                // Build up the pipeline
                Console.WriteLine("Pipeline is created.");

                // Send a trace to the pipeline
                Trace.TraceInformation("This is a message from trace . . .");
                MyEventSource.Log.Message("This is a message from EventSource ...");

                // Make a simple get request to bing.com just to generate some HTTP trace
                HttpClient client = new HttpClient();
                client.GetStringAsync("http://www.bing.com").Wait();

                // Check the result
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey(true);
            }
        }
    }
}
