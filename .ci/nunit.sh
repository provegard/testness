#!/bin/bash

set -e
set -x

mono --runtime=v4.0 .nuget/NuGet.exe install NUnit.Runners -Version 2.6.2 -o packages

runTest(){
   mono --runtime=v4.0 packages/NUnit.Runners.2.6.2/tools/nunit-console.exe -noxml -nodots -labels $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

#runTest $1 -exclude=Performance
runTest $@

exit $?
