/**
 * Interface
 * Defines ITest Interface which ever test driver must implement
 * 
 * FileName     : ITest.cs
 * Author       : Sahil Gupta
 * Date         : 25 September 2016 
 * Version      : 1.0
 */

using System;

namespace TestInterface
{
    public interface ITest
    {
        bool test();
        string getLog();
    }
}
