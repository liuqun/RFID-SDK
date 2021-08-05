#!/usr/bin/env bash
set -e
read="lib/reader.jar"
rxtx="lib/RXTXcomm.jar"
if [ -e $read ]
then
echo "$read 开始安装到本地"
mvn install:install-file -Dfile=lib/reader.jar -DgroupId=com.gg.reader -DartifactId=greader-api -Dversion=1.0 -Dpackaging=jar
else
echo "[$read]=>文件不存在"
fi
if [ -e $rxtx ]
then
echo "$rxtx 开始安装到本地"
mvn install:install-file -Dfile=lib/RXTXcomm.jar -DgroupId=org.rxtx -DartifactId=rxtx -Dversion=2.2pre2 -Dpackaging=jar
else
echo "[$rxtx]=> 文件不存在"
fi
