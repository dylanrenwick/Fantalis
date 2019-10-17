module.exports = {
    entry: './src/client/fantalis.js',
    mode: 'production',
    devtool: 'inline-source-map',
    resolve: {
      extensions: [ '.js' ]
    },
    output: {
        filename: 'Fantalis_Client.js'
    },
    performance: {
      hints: false
    }
}