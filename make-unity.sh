#!/bin/bash
set -e
UNITY=/Applications/Unity/Unity.app/Contents/MacOS/Unity
BASEDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

rm -f Distrib/ActionLib.unityPackage

$UNITY -batchmode \
 -projectPath $BASEDIR/SampleProject \
 -exportPackage Assets/ActionLib ../Distrib/ActionLib.unityPackage \
 -quit

 echo $?