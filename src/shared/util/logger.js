const fs = require('fs');

class logger {
    static get instance() {
        if (logger._instance === undefined) logger._instance = new logger();
        return logger._instance;
    }

    get logTimestamp() {
        let date = new Date();
        let tzOffset = date.getTimezoneOffset() / 60;
        tzOffset *= -1;
        return date.getFullYear()
            + '-' + (date.getMonth() + 1).toString().padStart(2, '0')
            + '-' + date.getDate().toString().padStart(2, '0')
            + ' ' + date.getHours().toString().padStart(2, '0')
            + ':' + date.getMinutes().toString().padStart(2, '0')
            + ':' + date.getSeconds().toString().padStart(2, '0')
            + '.' + date.getMilliseconds().toString().padStart(3, '0')
            + ' UTC' + (tzOffset >= 0 ? '+' : '') + tzOffset;
    }

    get detailColor() { return logger.logLevelColors["DETAIL"]; }
    get dateColor() { return logger.logLevelColors["DATE"]; }
    get arrowColor() { return logger.logLevelColors["ARROW"]; }
    get resetColor() { return logger.logLevelColors["RESET"]; }
    get logLevelLimit() {
        if (typeof(logger.logLevelLimit) === 'string') {
            logger.logLevelLimit = logger.logLevels[logger.logLevelLimit];
        }
        return logger.logLevelLimit;
    }

    log(msg, logLevel = 3) {
        if (logLevel > this.logLevelLimit) return;
        let lines = msg.split('\n');
        let output = this.formatMessage(lines.shift(), logLevel);
        if (lines.length >= 1) {
            for (let i = 0; i < lines.length; i++) {
                let quarterSize = Math.floor(logger.lineSpacingSize / 4);
                let remainderSize = logger.lineSpacingSize - quarterSize * 4;
                output += ' '.repeat(quarterSize * 3 - 1 + remainderSize) + this.arrowColor + '-' + ' '.repeat(quarterSize) + this.resetColor + lines[i];
            }
        }
        if (logger.logToConsole) console.log(output);
        if (logger.logToFile && logger.filePath.length > 0) {
            fs.appendFile(logger.filePath, output);
        }
    }

    formatMessage(msg, logLevel) {
        return this.detailColor + '[' + this.dateColor
             + this.logTimestamp + this.detailColor + ']['
             + logger.logLevelColors[logLevel] + logger.logLevels[logLevel]
             + this.detailColor + ']' + this.arrowColor + '> '
             + this.resetColor + msg;
    }
}

logger.logLevels = {
    LOG_CRT: 0,
    0: "LOG_CRT",
    LOG_ERR: 1,
    1: "LOG_ERR",
    LOG_WRN: 2,
    2: "LOG_WRN",
    LOG_INF: 3,
    3: "LOG_INF",
    LOG_DBG: 4,
    4: "LOG_DBG"
};

logger.logLevelColors = [
    constructANSICode(1, [1]),
    constructANSICode(1),
    constructANSICode(3),
    constructANSICode(-30),
    constructANSICode(6)
];
logger.logLevelColors["DATE"] = constructANSICode(3);
logger.logLevelColors["RESET"] = constructANSICode(-30);
logger.logLevelColors["DETAIL"] = constructANSICode(2);
logger.logLevelColors["ARROW"] = constructANSICode(3);

logger.lineSpacingSize = 8;

logger.logLevelLimit = 4;

function constructANSICode(color, flags = []) {
    return `\u001b[${color + 30}${flags.length > 0 ? ';' + flags.reduce((a,b) => `${a}${b}`) : ''}m`;
}

module.exports = logger;