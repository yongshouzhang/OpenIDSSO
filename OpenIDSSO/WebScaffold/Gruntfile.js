var path = require("path");
var fs = require("fs");
module.exports = function (grunt) {

    grunt.initConfig({
        less: {
            options: {
                paths: "lib",
            },
            build: {
                cwd:"less",
                src: "*.less",
                dest: "../Content/css/",
                expand: true,
                ext: ".css",
                filter: function (filepath) {
                    var dst = path.resolve("../Content/css/", path.basename(filepath, ".less") + this.ext);
                    try {
                        if (fs.existsSync(dst))
                            fs.unlinkSync(dst);
                    } catch (e) {
                        console.log(e.message);
                    }
                    return true;
                }
            }
        },
        watch: {
            files: "less/*.less",
            task:["less:build"]
        },
        clean: {
        }

    });
    grunt.loadNpmTasks("grunt-contrib-less");
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.registerTask("less2css", ["less:build"]);
}