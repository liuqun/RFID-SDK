// 命令行安装jar包到本地
mvn install:install-file -Dfile=lib/reader.jar -DgroupId=com.gg.reader -DartifactId=greader-api -Dversion=1.0 -Dpackaging=jar
// 将对应系统版本静态库文件放到jre环境.

//脚本安装install.sh或者install.cmd,请确保文件路径正确
