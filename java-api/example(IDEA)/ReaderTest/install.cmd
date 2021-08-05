@echo off
CHCP 65001
set read=lib\reader.jar
if exist %read% (
    	call mvn install:install-file -Dfile=lib/reader.jar -DgroupId=com.gg.reader -DartifactId=greader-api -Dversion=1.0 -Dpackaging=jar
    	echo ===========================%read%安装完成==============================
    ) else (
        color 04
    	echo %read%不存在
    )
pause
exit

