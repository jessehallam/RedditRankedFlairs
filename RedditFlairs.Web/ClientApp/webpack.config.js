const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

const isDevMode = process.env.NODE_ENV === 'development';

const DIST_PATH = path.resolve(__dirname, '../wwwroot/dist');
const SOURCE_PATH = __dirname;

module.exports = {
    entry: {
        main: [
            '@babel/polyfill',
            path.join(SOURCE_PATH, 'index')
        ]
    },

    mode: isDevMode ? 'development' : 'production',

    module: {
        rules: [
            {
                test: /\.tsx?/,
                use: [
                    { 
                        loader: 'babel-loader',
                        options: {
                            plugins: [
                                "react-hot-loader/babel"
                            ],
                            presets: [
                                '@babel/preset-env'
                            ]
                        }
                    },
                    {
                        loader: 'ts-loader'
                    }
                ]
            },
            {
                test: /\.s?css$/,
                use: [
                    { loader: 'style-loader' },
                    { loader: 'css-loader' },
                    { loader: 'sass-loader' }
                ]
            }
        ]
    },

    output: {
        filename: 'main.js',
        path: DIST_PATH,
        publicPath: '/dist/'
    },

    plugins: [
        new HtmlWebpackPlugin({
            template: path.join(SOURCE_PATH, 'index.html')
        })
    ],

    resolve: {
        extensions: ['.js', '.jsx', '.ts', '.tsx', '.json']
    }
}