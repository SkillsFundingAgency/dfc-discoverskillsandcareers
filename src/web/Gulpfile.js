/* eslint-disable no-console */
"use strict";

// requires

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    sass = require("gulp-sass"),
    eslint = require("gulp-eslint"),
    nunjucks = require('gulp-nunjucks-render'),
    connect = require('gulp-connect'),
    mocha = require('gulp-mocha'),
    sassLint = require('gulp-sass-lint');

// paths

var paths = {
    src: "src/",
    dist: "dist/",
    temp: ".temp/",
    buildScript: "build-script/"
};

// paths – input

paths.html = paths.src + "templates/**/*.html";
paths.scss = paths.src + "scss/**/*.scss";
paths.js = paths.src + "js/**/*.js";
paths.minJs = paths.src + "js/**/*.min.js";
paths.test = paths.buildScript + "test.js"

// paths - output

paths.css = paths.temp + "css/**/*.css";

paths.minCss = paths.temp + "css/**/*.min.css";
paths.concatMinCssDest = paths.dist + "css/site.min.css";
paths.concatJsDest = paths.temp + "js/site.js";
paths.concatMinJsDest = paths.dist + "js/site.min.js";
paths.assetsDest = paths.dist + "assets/";
paths.cssDest = paths.assetsDest + "css/";

// tasks

gulp.task('assets', function () {
    return gulp.src(['./node_modules/govuk-frontend/assets/**/*'])
        .pipe(gulp.dest(paths.assetsDest));
});

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatMinJsDest + "", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.css, cb);
    rimraf(paths.concatMinCssDest, cb);
});

gulp.task("clean:assets", function (cb) {
    rimraf(paths.assetsDest, cb);
});

gulp.task("sass", function () {
    return gulp.src(paths.scss)
        .pipe(sassLint())
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError())
        .pipe(sass({
            includePaths: 'node_modules'
        }))
        .pipe(gulp.dest(paths.cssDest))
        .pipe(connect.reload());
});

gulp.task("js", function () {
    return gulp.src([paths.js], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(gulp.dest("."));
});

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatMinJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatMinCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("eslint", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task('html', function() {
    return gulp.src(paths.html)
        .pipe(nunjucks({
          path: ["node_modules/govuk-frontend/", "node_modules/govuk-frontend/components/"]
        }))
        .pipe(gulp.dest(paths.dist))
        .pipe(connect.reload());
});

gulp.task('connect', function() {
  connect.server({
    root: paths.dist,
    port: 3000,
    livereload: true
  });
});

gulp.task('test', function() {
    gulp.src(paths.test, {read: false})
        .pipe(mocha())
        .on('error', () => {
            process.exit(1);
        });
});
 
// watches

gulp.task("css:watch", function () {
    gulp.watch([paths.css], gulp.series("min:css"));
});

gulp.task("sass:watch", function () {
    gulp.watch(paths.scss, gulp.series("sass", "test"));
});

gulp.task("eslint:watch", function () {
    gulp.watch([paths.js], gulp.series("eslint"));
});

gulp.task("js:watch", function () {
    gulp.watch([paths.js], gulp.series("js"));
});

gulp.task("html:watch", function () {
    gulp.watch([paths.html], gulp.series("html", "test"));
});

// commands

gulp.task("clean", gulp.parallel("clean:js", "clean:css", "clean:assets"));
gulp.task("min", gulp.parallel("min:js", "min:css"));

gulp.task("dev",
    gulp.series(
        "clean",
        "assets",
        "sass",
        "js",
        "html",
        "min:css",
        gulp.parallel(
            "html:watch",
            "css:watch",
            "sass:watch",
            "js:watch",
            "eslint:watch",
            "connect"))
);

gulp.task("prod",
    gulp.series(
        "clean",
        "assets",
        "sass",
        "html",
        "eslint",
        "min")
);