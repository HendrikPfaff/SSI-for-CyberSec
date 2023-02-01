# A small demo for Identity & Access Management via SSI

This project can be seperated into two parts.
There is the .NET MAUI Holder app, which is meant to be used by the user to install and use on a (mobile) system.
On the other side there is the Issuer/Verifyer Webapps, meant to run on the [Hyperledger Indy](https://github.com/hyperledger/indy-sdk) infrastructure and to be installed on a VPS (or similar server).

With both parts in place, a Demo-Student can connect to a Demo-University and get his degree issued while a Demo-company can send proof-requests to verify his degree attributes.