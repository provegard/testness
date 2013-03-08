// Copyright (C) 2011-2013 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.
namespace TestNess.Lib.Reporting
{
    public interface IReportReceiver
    {
        void GenerateReport(string contents);
        void GenerateSupplementaryFile(string fileName, string contents);
    }
}
