const gulp = require('gulp');
const rimraf = require('rimraf');
const ts = require('gulp-typescript');
const sm = require('gulp-sourcemaps');

const webpack = require('webpack-stream');
const webpackConf = require('./webpack.config.js');

function buildTypescript(done) {
    let tsProject = ts.createProject('tsconfig.json');

    return tsProject.src()
        .pipe(sm.init())
        .pipe(tsProject()).js
        .pipe(sm.write('.'))
        .pipe(gulp.dest('./bin/js/'))
        .on('end', () => gulp.src('./src/**/*.json')
            .pipe(gulp.dest('./bin/js/'))
            .on('end', done)
        );
}

function buildClient(done) {
    return gulp.src('./bin/js/client/**/*.js')
        .pipe(webpack(webpackConf))
        .pipe(gulp.dest('./bin/client'))
        .on("end", () => gulp.src("./bin/js/client/**/*.json")
            .pipe(gulp.dest("./bin/client"))
            .on("end", () => {
                rimraf.sync('./dist');
                done();
            })
        );
}

function buildServer(done) {
    return gulp.src("./bin/js/server/**/*.js")
        .pipe(gulp.dest('./bin/server'))
        .on("end", () => gulp.src("./bin/js/server/**/*.json")
            .pipe(gulp.dest("./bin/server"))
            .on("end", () => gulp.src("./bin/js/shared/**/*.js")
                .pipe(gulp.dest("./bin/server"))
                .on("end", done)
            )
        );
}

function clean(done) {
    rimraf.sync('./bin');
    done();
}

exports.buildTypescript = buildTypescript;
exports.buildClient = buildClient;
exports.buildServer = buildServer;
exports.build = gulp.series(buildTypescript, buildClient, buildServer);
exports.clean = clean;
exports.default = gulp.series(exports.clean, exports.build);
