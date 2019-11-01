const server = require('./server.js');

const config = require('./config.json');

let optional = ['debug', 'log'];
let required = ['version', 'server'];

for (let req of required) {
    if (!config[req]) throw new Error("Invalid or corrupt config, '" + req + "' could not be found!");
}
for (let opt of optional) {
    if (!config[opt]) config[opt] = {};
}

const gameServer = new server(config);

gameServer.start();