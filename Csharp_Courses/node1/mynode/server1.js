var formidable = require('formidable'),
    http = require('http');
    util = require('util');

var conract_hash = "0x4e4d5a78ea08de7ed43c578a4eeda07fca042b4b"

function notErrorResponse(str){
    if(str.indexOf("error") > -1 || str.indexOf("Wrong") > -1)
        return 0;
    return 1;
}

function hex_to_ascii(str1)
{
    var hex  = str1.toString();
    var str = '';
    for (var n = 0; n < hex.length; n += 2) {
        if (n > 0 && String.fromCharCode(parseInt(hex.substr(n-2, 2), 16)) == ' ' && String.fromCharCode(parseInt(hex.substr(n, 2), 16)) == '|')
            str += ' ';
        else
            if (n > 0 && String.fromCharCode(parseInt(hex.substr(n, 2), 16)) == '|')
                str += ', '
            else
                str += String.fromCharCode(parseInt(hex.substr(n, 2), 16));
    }
    return str;
}

var request = require('request');
global.mybody = "empty";

function postRawReq(tx){
    return new Promise(function(resolve, reject){
        request.post( //options, callback
            'http://localhost:30332/',
            {
                json: {
                    "jsonrpc": "2.0",
                    "method": "sendrawtransaction",
                    "params": [tx],
                    "id": 1
                }
            },
            function (error, response, body) { //thats the callback function
                if (!error && response.statusCode === 200) {
                    console.log("--------RAW-------");
                    console.log("rpc node response body result (boolean):::");
                    console.log(body.result);
                    console.log("--------RAW-------");
                    console.log("********RAW*******");
                    console.log("rpc node raw all body:::");
                    console.log(body.result);
                    console.log("********RAW*******");
                    resolve(body);
                }
                else {
                    console.log("post error");
                    reject(error);
                }
            }
        )
    })
}



function postreq(func, courseNum, ID, offer, password) {
    return new Promise(function(resolve, reject){
        request.post( //options, callback
            'http://localhost:30332/',
            {
                json: {
                    "jsonrpc": "2.0",
                    "method": "invokefunction",
                    "params": [
                        conract_hash, //smart contract hash
                        func,
                        [
                            {
                                "type": "String",
                                "value": courseNum
                            },
                            {
                                "type": "String",
                                "value": ID
                            },
                            {
                                "type": "String",
                                "value": offer
                            },
                            {
                                "type": "String",
                                "value": password
                            },
                        ]
                    ],
                    "id": 3
                },
                timeout: 1500,
            },
            function (error, response, body) { //thats the callback function
                if (!error && response.statusCode === 200) {
                    console.log("------------------");
                    console.log("rpc node response body result stack:::");
                    if (body.result.stack.length > 0)
                        console.log(hex_to_ascii(body.result.stack[0].value));
                    console.log("------------------");
                    console.log("******************");
                    console.log("rpc node response body result tx:::");
                    console.log(body.result.tx);
                    console.log("******************");
                    console.log("------------------");
                    console.log("rpc node response body result state:::");
                    console.log(body.result.state);
                    console.log("------------------");
                    resolve(body);
                }
                else {
                    console.log("post error");
                    reject(error);
                }
            }
        )
    })
}

http.createServer(function(req, res) {
    if (req.method.toLowerCase() === 'post') {
        // parse an operation
        let form = new formidable.IncomingForm();
        form.parse(req, function (err, fields) {
            console.log("parsing");
            let tx = "empty";
            let ID = fields.ID;
            let want = fields.want;
            let offer = fields.offer;
            let password = fields.password;
            let func = req.url;
            let changeStorage =0;
            func = func.replace('/','');
            if (func == "regReq" || func == "remove" || func == "offer")
                changeStorage=1;
            if (func == "refresh")
                want = "1";
            console.log("Envoking smart contract (method=", func,") with ID:", ID, ". courseNum:", want, ". offer: ", offer, ". password:", password);
            let postPromise = postreq(func, want, ID, offer, password);
            postPromise.then(function(result) { //resolved
                console.log("+++++++++++++++");
                let blockChainRes = result.result.stack;
                tx = result.result.tx;
                console.log(blockChainRes);
                word1 = '';
                if (changeStorage == 1)
                    word1 = '<u>Simulation:</u><br>';
                for (i = 0; i < blockChainRes.length; i++) {
                    word1 += hex_to_ascii(blockChainRes[i].value + '<br><br>');
                }
                res.writeHead(200, {'content-type': 'text/html'});
                res.write('<style> div{font-size:35px; margin: 70px; border: 1px solid #4CAF50;}</style><div>' + word1 + '</div></>');
                console.log("+++++++++++++++");
                if (changeStorage == 1 && notErrorResponse(word1) == 1) {
                    console.log("Starting RawTransaction Invoke")
                    console.log("tx: "+tx)
                    let postRawPromise = postRawReq(tx);
                    postRawPromise.then(function (result) { //resolved
                            let blockChainRes2;
                            if (result.result == true)
                                blockChainRes2 = "Request sent success";
                            else
                                blockChainRes2 = "Request sent faild. <br>Try again in a few seconds";
                            res.write('<style> div{font-size:35px; margin: 70px; border: 1px solid #4CAF50;}</style><div>' + blockChainRes2 + '</div></>');
                            res.end();
                        },
                        function (err) { //error
                            console.log("Envoking Raw TX failed")
                            res.write("Envoking Raw TX failed")
                            console.log(err)
                            res.end();
                        });
                }
            },
                function (err) { //error
                    console.log("Envoking Smart Contract failed")
                    res.write("Envoking Raw TX failed")
                    console.log(err)
                    res.end();
                });
            console.log("end main function")
        });
    }
}).listen(3000,() => console.log('Listening on 3000'));
