/*
 *  FILENAME        : MyStopwatch.cs
 *  PROJECT         : MemoryCaching
 *  PROGRAMMER      : Jody Markic
 *  FIRST VERSION   : 2/21/2018
 *  DESCRIPTION     : This a wrapper class for the Stopwatch class it holds method for starting and stopping stopwatches
 *                    along with the ability to calculate elapsed ticks during a cycle of starting and stopping.
 *                    It lastly calculates an average tick and nanosecond duration for performance testing.
 */

 //included namespaces
using System;
using System.Diagnostics;

//project namespace
namespace MemoryCaching
{
    //
    //  CLASS       : MyStopwatch
    //  DESCRIPTION : This a wrapper class for the Stopwatch class it holds method for starting and stopping stopwatches
    //                along with the ability to calculate elapsed ticks during a cycle of starting and stopping.
    //                It lastly calculates an average tick and nanosecond duration for performance testing.
    //
    class MyStopwatch
    {
        private Stopwatch TotalStopwatch = null;
        private Stopwatch InstanceStopwatch = null;

        private long currentTicks;
        private long totalTicks;
        private long totalTimeTicks;

        //
        //  METHOD: MyStopwatch
        //  DESCRIPTION: Constructor for MyStopwatch
        //  PARAMETERS: n/a
        //  RETURNS: n/a
        //
        public MyStopwatch()
        {
            TotalStopwatch = new Stopwatch();
            InstanceStopwatch = new Stopwatch();
        }

        //
        //  METHOD: StartWatch
        //  DESCRIPTION: Starts a stopwatch based on the bool provided
        //  PARAMETERS: bool isTotal
        //  RETURNS: void
        //
        public void StartWatch(bool isTotal)
        {
            if (isTotal)
            {
                TotalStopwatch = Stopwatch.StartNew();
            }
            else
            {
                InstanceStopwatch  = Stopwatch.StartNew();
            }
        }

        //
        //  METHOD: ResetTotalTicks
        //  DESCRIPTION: Reset the total ticks a stopwatch has calculated
        //  PARAMETERS: n/a
        //  RETURNS: void
        //
        public void ResetTotalTicks()
        {
            totalTicks = 0;
        }

        //
        //  METHOD: StopWatch
        //  DESCRIPTION: Stops a stopwatch based on the bool provided
        //  PARAMETERS: bool isTotal
        //  RETURNS: void
        //
        public void StopWatch(bool isTotal)
        {
            if (isTotal)
            {
                TotalStopwatch.Stop();
            }
            else
            {
                InstanceStopwatch.Stop();
            }
        }

        //
        //  METHOD: TimeElapsed
        //  DESCRIPTION: calculates the ticks elapsed from when a stop watch has been started then stopped
        //  PARAMETERS: int currentIteration, bool isTotal
        //  RETURNS: void
        //
        public void TimeElapsed(int currentIteration, bool isTotal)
        {
            //string elapsedTime = "";
            if (currentIteration != 0)
            {

                if (!isTotal)
                {
                    currentTicks = InstanceStopwatch.ElapsedTicks;
                    totalTicks += currentTicks;
                }
                else
                {
                    totalTimeTicks = TotalStopwatch.ElapsedTicks;
                }
            }
        }

        //
        //  METHOD: AverageTime
        //  DESCRIPTION: Calculates the average ticks and nanoseconds that have passed since a stopwatch has been started then stopped
        //  PARAMETERS: long iterations, bool isCached
        //  RETURNS: string result
        //
        public string AverageTime(long iterations, bool isCached)
        {
            string result = "";
            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            long averageTicks = totalTicks / iterations;
            long averageNanoSeconds = ((totalTicks * nanosecPerTick) / iterations);

            if (!isCached)
            {
                result = String.Format("Average time without Cache:  {0} ticks = {1} nanoseconds", averageTicks.ToString(), averageNanoSeconds.ToString());
            }
            else
            {
                result = String.Format("Average time with Cache:  {0} ticks = {1} nanoseconds", averageTicks.ToString(), averageNanoSeconds.ToString());
            }

            return result;
        }
    }
}
