// 安装jar包到本地（或者执行项目根目录install.bat）
mvn install:install-file -Dfile=lib/reader.jar -DgroupId=com.gg.reader -DartifactId=greader-api -Dversion=1.0 -Dpackaging=jar
// 将对应系统版本静态库文件放到jre环境.
