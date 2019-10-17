const gulp = require('gulp');
const rimraf = require('rimraf');

const webpack = require('webpack-stream');
const webpackConf = require('./webpack.config.js');

function buildClient(done) {
    return gulp.src("./src/client/**/*.js")
        .pipe(webpack(webpackConf))
        .pipe(gulp.dest('./bin/client'))
        .on("end", () => gulp.src("./src/client/**/*.json")
            .pipe(gulp.dest("./bin/client"))
            .on("end", done)
        );
};

function buildServer(done) {
    return gulp.src("./src/server/**/*.js")
        .pipe(gulp.dest('./bin/server'))
        .on("end", () => gulp.src("./src/server/**/*.json")
            .pipe(gulp.dest("./bin/server"))
            .on("end", () => gulp.src("./src/shared/**/*.js")
                .pipe(gulp.dest("./bin/server"))
                .on("end", done)
            )
        );
};

function clean(done) {
    rimraf.sync('./bin/client');
    rimraf.sync('./bin/server');
    done();
}

exports.buildClient = buildClient;
exports.buildServer = buildServer;
exports.build = gulp.series(buildClient, buildServer);
exports.clean = clean;
exports.default = gulp.series(exports.clean, exports.build);
