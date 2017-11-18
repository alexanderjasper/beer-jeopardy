/// <binding AfterBuild='sass' />
var gulp = require("gulp"),
    fs = require("fs"),
    sass = require("gulp-sass");

gulp.task("sass", function () {
    return gulp.src('Styles/Main.scss')
        .pipe(sass())
        .pipe(gulp.dest('wwwroot/css'));
});