# Installation and SSI Operation
This step-by-step guide is meant to be a manual to reproduce our steps from installation and setting up the required software packages / containers, as well as to give an overview on the required API calls for operating SSI agents.

## Installation on VPS
Before we can start working with SSI agents, we need to set up the required ledger infrastructure and Docker containers.
Using a Linux (Virtual Private) Server for the installation and operation of this SSI Demo is highly recommended.
For the usage on personal computers changes in the process may need to be made.

### Install Docker and Docker Compse
If not already installed, install Docker and Docker Compose to your VPS by following [this guide](https://github.com/bcgov/von-network#running-the-network-on-a-vps). 
Root privileges may be required by some steps. 

```
apt install unzip
```

```
curl -fsSL get.docker.com -o get-docker.sh
```

```
sh get-docker.sh
```

```
curl -L https://github.com/docker/compose/releases/download/1.24.1/docker-compose-`uname -s`-`uname -m` -o /usr/local/bin/docker-compose
```

```
chmod +x /usr/local/bin/docker-compose
```

### Manage required Ports
To let the SSI agents communicate properly, the system firewall must allow communication on several ports.

!!! WARNING: This may compromise system security !!!
```
ufw allow in 5000/tcp && ufw allow in 9000/tcp && ufw allow in 9701:9708/tcp 
```

### Install the VON-network containers
The [VON-network](https://github.com/bcgov/von-network) creates the necessary ledger infrastructure for our SSI agents.
```
curl -L https://github.com/bcgov/von-network/archive/main.zip > bcovrin.zip && \
    unzip bcovrin.zip && \
    cd von-network-main && \
    chmod a+w ./server/
```

```
./manage build
```

Set `[Server IP]` to your specific one. 
```
./manage start [SERVER IP] WEB_SERVER_HOST_PORT=9000 "LEDGER_INSTANCE_NAME=My Indy Ledger" &
```

### Register agent DID
To give our future *Issuer* and *Verifier* a distinct identity within the ledger, it is necessary to create a [DID](https://en.wikipedia.org/wiki/Decentralized_identifier) for each of them.

In your browser, call `[Server IP]:9000` to access the web interface of the VON-network.

In the Form "Authenticate new DID" enter the seeds for your *Issuer* and *Verifier* (e.g. the seeds below) and press "Register DID".
```
Example issuer seed: 0923456ERFDZSXCVTYUO9986OREDFBBB
Example verifier seed: 0923456ERFDZSXCVTYUO9986VERIFIER
```

### Create agents
Now we need to create our three SSI agents *Issuer*, *Verifier* and *Holder* as docker containers.
It is important to adjust the `[Server IP]` (and `[Seed]`-argument for *Issuer* and *Verifier*) according to your respective values.

Creating the *Issuer*:
```
docker run --rm -ti --name issuer --net=von_von -p 8050:8050 -p 8051:8051 bcgovimages/aries-cloudagent:py36-1.14-1_0.5.1 start -it http 0.0.0.0 8050 -ot http -e http://[Server IP]:8050 -l issuer.Agent --wallet-type indy --seed [Seed] --genesis-url http://[Server IP]:9000/genesis --admin 0.0.0.0 8051 --trace-target log --trace-tag acapy.events --trace-label issuer.Agent.trace --admin-insecure-mode --auto-accept-invites --auto-accept-requests --auto-respond-messages --auto-ping-connection
```

Creating the *Verifier*:
```
docker run --rm -ti --name verifier --net=von_von -p 8070:8070 -p 8071:8071 bcgovimages/aries-cloudagent:py36-1.14-1_0.5.1 start -it http 0.0.0.0 8070 -ot http -e http://[Server IP]:8070 -l verifier.Agent --wallet-type indy --seed [Seed] --genesis-url http://[Server IP]:9000/genesis --admin 0.0.0.0 8071 --trace-target log --trace-tag acapy.events --trace-label verifier.Agent.trace --admin-insecure-mode --auto-accept-invites --auto-accept-requests --auto-respond-messages --auto-ping-connection   
```

Creating the *Holder*:
```
docker run --rm -ti --name holder --net=von_von -p 8090:8090 -p 8091:8091 bcgovimages/aries-cloudagent:py36-1.14-1_0.5.1 start -it http 0.0.0.0 8090 -ot http -e http://[Server IP]:8090  -l holder.Agent --wallet-type indy --genesis-url http://[Server IP]:9000/genesis --admin 0.0.0.0 8091 --trace-target log --trace-tag acapy.events --trace-label holder.Agent.trace --admin-insecure-mode --auto-accept-invites --auto-accept-requests --auto-store-credential --debug-credentials
```

### Calling agents Swagger Endpoints
Now that we created the required infrastructure and agent containers, we can call their Endpoints in our browser to make API calls.

Issuer: `http://[Server IP]:8051`

Verifier: `http://[Server IP]:8071`

Holder: `http://[Server IP]:8091`

## Operation of the SSI process
Now with the endpoints in place, we can make [REST](https://en.wikipedia.org/wiki/Representational_state_transfer)-API calls to perform the actions of the SSI process.
We first establish the connections between our agents `Issuer <-> Holder <-> Verifier`, send a credential from *Issuer* to *Holder* and the let the *Verifier* validate it.

### Establish a connection between Issuer and Holder
**1. Create Connection Invitation (Issuer)**

`POST` to `/connections/create-invitation`

Response:
```
{
    "connection_id": "57057bae-5c9f-42f1-bc26-693db7c41921",
	"invitation": {
	    "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation",
		"@id": "c1eaba9a-317f-4501-b071-c129a49d2315",
		"label": "issuer.Agent",
		"recipientKeys": [
		      "54ah9aqsjyuvqHymYJjFQQk3gqh3dnTHzR5qjENw8pb4"
		],
		"serviceEndpoint": "http://[Server IP]:8050"
  	},
  	"invitation_url": "http://[Server IP]:8050?c_i=eyJAdHlwZSI6ICJkaWQ6c292OkJ6Q2JzTlloTXJqSGlxWkRUVUFTSGc7c3BlYy9jb25uZWN0aW9ucy8xLjAvaW52aXRhdGlvbiIsICJAaWQiOiAiYzFlYWJhOWEtMzE3Zi00NTAxLWIwNzEtYzEyOWE0OWQyMzE1IiwgImxhYmVsIjogImlzc3Vlci5BZ2VudCIsICJyZWNpcGllbnRLZXlzIjogWyI1NGFoOWFxc2p5dXZxSHltWUpqRlFRazNncWgzZG5USHpSNXFqRU53OHBiNCJdLCAic2VydmljZUVuZHBvaW50IjogImh0dHA6Ly8zOC4yNDIuMjQ4LjIzNzo4MDUwIn0="
}
```

**2. Accept Connection Invitation (Holder)**

`POST` to `/connections/receive-invitation` with the "invitation" JSON-object from the previous *Issuer*-call as parameter
(Note: We set the auto-accept flag when we started the container, otherwise the *Holder* needs to send another message to accept it):
```
{
    "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation",
	"@id": "c1eaba9a-317f-4501-b071-c129a49d2315",
	"label": "issuer.Agent",
	"recipientKeys": [
		"54ah9aqsjyuvqHymYJjFQQk3gqh3dnTHzR5qjENw8pb4"
	],
	"serviceEndpoint": "http://[Server IP]:8050"
}
```

Response:
```
{
    "invitation_mode": "once",
    "invitation_key": "54ah9aqsjyuvqHymYJjFQQk3gqh3dnTHzR5qjENw8pb4",
    "updated_at": "2023-01-24 22:25:30.290765Z",
    "their_label": "issuer.Agent",
    "request_id": "3ff5876c-8ab8-4fea-8e4f-21b63df708d9",
    "routing_state": "none",
    "accept": "auto",
    "state": "request",
    "initiator": "external",
    "connection_id": "e2a1eb1e-5756-4930-883f-2915f1540c85",
    "my_did": "XK5Asg9MZZjugRwWug42f3",
    "created_at": "2023-01-24 22:25:30.256607Z"
}
```

### Issue a claim

**1. Create Schema on ledger (Issuer)**

`POST` to `/schemas` with the claims we want to have in our schema as parameter:
```
{
    "schema_name": "bachelorzeugnis_schema",
    "schema_version": "1.0",
    "attributes": [
        "student_name",
        "studiengang",
        "abschlussnote"
    ]
}
```

Response:
```
{
  "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
  "schema": {
    "ver": "1.0",
    "id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
    "name": "bachelorzeugnis_schema",
    "version": "1.0",
    "attrNames": [
      "student_name",
      "studiengang",
      "abschlussnote"
    ],
    "seqNo": 10
  }
}
```

**2. Create Credential Definition (Issuer)**

`POST` to `/credential-definition` with the "schema_id" set from the previous call as parameter:
```
{
    "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
    "support_revocation": true,
    "tag": "default"
}
```

Response:
```
{
    "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default"
}
```

**3. Create a Revocation Registry for the Credential Definition (Issuer)**

`POST` to `/revocation/create-registry` with the previously created "credential_definition":

```
{
  "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
  "max_cred_num": 100,
  "issuance_by_default": true
}
```

Response:
```
{
  "result": {
    "revoc_reg_entry": {
      "ver": "1.0",
      "value": {
        "accum": "21 13E362FD6107AC1CE003D0910F25DE17BBD6D45A8CDB2AD38B4F8320EB9EC326E 21 134D4FF84B5DB3E0FE2D819EEE22E568E3CD2F642790BA696F21D925369F14DBE 6 7570093A0ACB0737FCF02168B2CF57FA98E19AAABAA620DFE2461C3BB61ACA58 4 0CB362BC7505CAD83487715137B6F37BBA3228CF81EC8BBB742E452194A6BCDC 6 6B0C470DEF3864116892A1543F78D2EBF0B0A84AE5A04D5E7A3B3A881A602895 4 1237B204260AB7AFB47AC81624F94FBC4B2885A2469FE8D28C831751623594F3"
      }
    },
    "tag": "bfaf60fa-1862-40fe-a79d-98731fb5a293",
    "tails_hash": "2y5tKdt7vSbM2j28AxAktSkhQ2znnYMuZrC9uoRHYCRj",
    "revoc_reg_def": {
      "ver": "1.0",
      "id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:12:default:CL_ACCUM:bfaf60fa-1862-40fe-a79d-98731fb5a293",
      "revocDefType": "CL_ACCUM",
      "tag": "bfaf60fa-1862-40fe-a79d-98731fb5a293",
      "credDefId": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:12:default",
      "value": {
        "issuanceType": "ISSUANCE_BY_DEFAULT",
        "maxCredNum": 100,
        "publicKeys": {
          "accumKey": {
            "z": "1 2274A955846F7F32B2EFFDBD76EC9D5F5BF0F1E13A47F20E64BFC4326DE5142A 1 1B961D3F85B0930DFCA0EEC4CCDE384F0FD23B23FB0293240824557FA0559EC6 1 148E8DF0B5F8B605A72CDDF43C0A04AB799D8DE99081462C2ED68C03E63E375B 1 14B2CCE47BA8F11AAB0EAE343F1D33AF05000A9C8A0AD3CAB3C06EF92F4458DE 1 160168DA5B7EE3B2F1111B54E8137461D03B6A45D859DA0F8BDC91569A5DEF32 1 1F269F573750A02B7344F7F2B9F10CFA98BC32241ED2C97AD518546EC08010FA 1 02AAAA7DA85CCFA85DB6B9FDA644924DB09E445FF06B688C9E6B89B31E0F7276 1 00D226F721FDA957F94F7C13F39341CFF5A143D41AC764AE34890FD5FF0763A7 1 2239FFE7830D6FC658F7DA42D8176622781FA275EB9F2574D5AE54D6E6C6F900 1 097DFA7E43755796AC7A9A607763BC096E7F65EC32D217462DB9175761853A7E 1 1F1F51CBD17641CAF5EB20DECDFEE4B167EA13373B73FFDB764D0D436A722AE0 1 06FDD623447A5684483AF4A0780A7C716C6F41038C80B346EC5DA863E127A1C0"
          }
        },
        "tailsHash": "2y5tKdt7vSbM2j28AxAktSkhQ2znnYMuZrC9uoRHYCRj",
        "tailsLocation": "/tmp/tmp7lwdl5lurevoc/2y5tKdt7vSbM2j28AxAktSkhQ2znnYMuZrC9uoRHYCRj"
      }
    },
    "max_cred_num": 100,
    "revoc_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:12:default:CL_ACCUM:bfaf60fa-1862-40fe-a79d-98731fb5a293",
    "pending_pub": [],
    "revoc_def_type": "CL_ACCUM",
    "record_id": "bfaf60fa-1862-40fe-a79d-98731fb5a293",
    "state": "generated",
    "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:12:default",
    "tails_local_path": "/tmp/tmp7lwdl5lurevoc/2y5tKdt7vSbM2j28AxAktSkhQ2znnYMuZrC9uoRHYCRj",
    "issuance_type": "ISSUANCE_BY_DEFAULT",
    "created_at": "2023-01-29 23:54:21.095862Z",
    "updated_at": "2023-01-29 23:54:22.368428Z",
    "issuer_did": "A9A3zmbBnPT6RcrKvTf9q7"
  }
}
```

**4. Update Revocation Registry URI**

`PUT` to `/revocation/registry/{revocation_registry_id}` with the "revoc_reg_id" from the previous step:
```
{
  "tails_public_uri": "[Server IP]:8051/revocation/registry/{revocation_registry_id}/tails-file"
}
```

Response:
```
{
    "result": {
        "revoc_reg_entry": {
            "ver": "1.0",
            "value": {
                "accum": "21 12B9CAC10BCF3C7FC77280F46EBAE3461C2B69761B9663113C6B6C232105DE5C2 21 12B3318A1B14D7FB8D11FE2F537763BE50D3959A7D72216FF2330C59480A629A4 6 8A76872F142ADC4CAC87BF09E57AFB29CD9FDCE47A3AD14CC0441D580C531B79 4 19616724B70CCF68F8AB3220F15E41EED41B94DC2BB91DB2DE23FF859A8A2662 6 6C9BD9306E972EB0D97ED4DEEB4D1E7B967871BA283AEF22727CE30746729518 4 2C0E0D7FF0242B674EE94A8C0D9B0E8A1DBC742524E686B8B59352B4D6A2B1F1"
            }
        },
        "tag": "1690937d-d81f-4688-970e-e7eeebef94f1",
        "tails_hash": "3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
        "revoc_reg_def": {
            "ver": "1.0",
            "id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
            "revocDefType": "CL_ACCUM",
            "tag": "1690937d-d81f-4688-970e-e7eeebef94f1",
            "credDefId": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
            "value": {
                "issuanceType": "ISSUANCE_BY_DEFAULT",
                "maxCredNum": 100,
                "publicKeys": {
                    "accumKey": {
                        "z": "1 1DBA8718A24D9360911EE781ACAE8FEC7B99404549EF006883A0B0F1922F812C 1 18D6E18FB0C5D8EBA04D6A61D0F26DE01FA09BE0A6B40E65B1D8CDF93FEF163D 1 065E794A6EA8AD5BE83F6CCEF869BA42525E39DED3D74A974E1F2E703F83565A 1 1B58AF48D3C965723D17D60495FEBF4E7B7DDC45619711E57351A54F20941F75 1 0DF48638ADC0AACDB303D3656FCF5592D97DB2B70019828AFD1CB5C6B7D2CEA5 1 004500748D9A05EA70911107A4A152B3B2487E158409304264B5B4646739F1FC 1 06C725E55B2BC234DCAB09DA08358093D5EF4B1E540B7F1DAB0246586221D0AB 1 0567A3D3DCA6A519F197329BE74501BD713CF2A16A6167600E0D6AE639B02C73 1 2287A5E7EF93A76EED3D657BBBFD3248BE8CCDF69F239B19863D04B2C93DD4CA 1 1AE437D7E8D36770D81B0DBA6977A3C275243120A8E108B6AE2D848D6E1BADB7 1 0FA57AEB3B25D0CB2DDC5C304301C49B5CF375B802CA17B42C22DC149C7B6D00 1 0875C6BEB6AD7EF63B3DF51B6E1F3FBF04784C649984E04F2681D1A01908203A"
                    }
                },
                "tailsHash": "3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
                "tailsLocation": "http://38.242.248.237:5000/revocation/registry/WgWxqztrNooG92RXvxSTWv:4:WgWxqztrNooG92RXvxSTWv:3:CL:20:tag:CL_ACCUM:0/tails-file"
            }
        },
        "max_cred_num": 100,
        "revoc_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
        "pending_pub": [],
        "revoc_def_type": "CL_ACCUM",
        "record_id": "1690937d-d81f-4688-970e-e7eeebef94f1",
        "state": "generated",
        "tails_public_uri": "http://38.242.248.237:5000/revocation/registry/WgWxqztrNooG92RXvxSTWv:4:WgWxqztrNooG92RXvxSTWv:3:CL:20:tag:CL_ACCUM:0/tails-file",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "tails_local_path": "/tmp/tmp7lwdl5lurevoc/3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
        "issuance_type": "ISSUANCE_BY_DEFAULT",
        "created_at": "2023-01-30 00:06:58.733164Z",
        "updated_at": "2023-01-30 05:59:23.176379Z",
        "issuer_did": "A9A3zmbBnPT6RcrKvTf9q7"
    }
}
```

**5. Publish Revocation Registry**

`POST` to `/revocation/registry/{id}/publish` with the id of the created revocation registry:

Response:
```
{
    "result": {
        "revoc_reg_entry": {
            "ver": "1.0",
            "value": {
                "accum": "21 12B9CAC10BCF3C7FC77280F46EBAE3461C2B69761B9663113C6B6C232105DE5C2 21 12B3318A1B14D7FB8D11FE2F537763BE50D3959A7D72216FF2330C59480A629A4 6 8A76872F142ADC4CAC87BF09E57AFB29CD9FDCE47A3AD14CC0441D580C531B79 4 19616724B70CCF68F8AB3220F15E41EED41B94DC2BB91DB2DE23FF859A8A2662 6 6C9BD9306E972EB0D97ED4DEEB4D1E7B967871BA283AEF22727CE30746729518 4 2C0E0D7FF0242B674EE94A8C0D9B0E8A1DBC742524E686B8B59352B4D6A2B1F1"
            }
        },
        "tag": "1690937d-d81f-4688-970e-e7eeebef94f1",
        "tails_hash": "3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
        "revoc_reg_def": {
            "ver": "1.0",
            "id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
            "revocDefType": "CL_ACCUM",
            "tag": "1690937d-d81f-4688-970e-e7eeebef94f1",
            "credDefId": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
            "value": {
                "issuanceType": "ISSUANCE_BY_DEFAULT",
                "maxCredNum": 100,
                "publicKeys": {
                    "accumKey": {
                        "z": "1 1DBA8718A24D9360911EE781ACAE8FEC7B99404549EF006883A0B0F1922F812C 1 18D6E18FB0C5D8EBA04D6A61D0F26DE01FA09BE0A6B40E65B1D8CDF93FEF163D 1 065E794A6EA8AD5BE83F6CCEF869BA42525E39DED3D74A974E1F2E703F83565A 1 1B58AF48D3C965723D17D60495FEBF4E7B7DDC45619711E57351A54F20941F75 1 0DF48638ADC0AACDB303D3656FCF5592D97DB2B70019828AFD1CB5C6B7D2CEA5 1 004500748D9A05EA70911107A4A152B3B2487E158409304264B5B4646739F1FC 1 06C725E55B2BC234DCAB09DA08358093D5EF4B1E540B7F1DAB0246586221D0AB 1 0567A3D3DCA6A519F197329BE74501BD713CF2A16A6167600E0D6AE639B02C73 1 2287A5E7EF93A76EED3D657BBBFD3248BE8CCDF69F239B19863D04B2C93DD4CA 1 1AE437D7E8D36770D81B0DBA6977A3C275243120A8E108B6AE2D848D6E1BADB7 1 0FA57AEB3B25D0CB2DDC5C304301C49B5CF375B802CA17B42C22DC149C7B6D00 1 0875C6BEB6AD7EF63B3DF51B6E1F3FBF04784C649984E04F2681D1A01908203A"
                    }
                },
                "tailsHash": "3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
                "tailsLocation": "[Server IP]:5000/revocation/registry/WgWxqztrNooG92RXvxSTWv:4:WgWxqztrNooG92RXvxSTWv:3:CL:20:tag:CL_ACCUM:0/tails-file"
            }
        },
        "max_cred_num": 100,
        "revoc_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
        "pending_pub": [],
        "revoc_def_type": "CL_ACCUM",
        "record_id": "1690937d-d81f-4688-970e-e7eeebef94f1",
        "state": "active",
        "tails_public_uri": "[Server IP]:5000/revocation/registry/WgWxqztrNooG92RXvxSTWv:4:WgWxqztrNooG92RXvxSTWv:3:CL:20:tag:CL_ACCUM:0/tails-file",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "tails_local_path": "/tmp/tmp7lwdl5lurevoc/3tKuGAvf1mBgZcsiJjUARRZSVDoMXpYmxYpXuFVfLzfY",
        "issuance_type": "ISSUANCE_BY_DEFAULT",
        "created_at": "2023-01-30 00:06:58.733164Z",
        "updated_at": "2023-01-30 06:00:14.812170Z",
        "issuer_did": "A9A3zmbBnPT6RcrKvTf9q7"
    }
}
```

**6. Send Credential Offer to Holder (Issuer)**

`POST` to `/issue-credential/send-offer` with "connection_id" set to the established connection with the *Holder*, "cred_def_id" set to the previously created credential definition and the desired "attributes" set to the desired values:
```
{
    "connection_id": "57057bae-5c9f-42f1-bc26-693db7c41921",
    "auto_issue": true,
    "trace": true,
    "comment": "string",
    "credential_preview": {
        "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
        "attributes": [
            {
                "name": "student_name",
                "value": "Max Mustermann"
            },
            {
                "name": "studiengang",
                "value": "Informatik"
            },
            {
                "name": "abschlussnote",
                "value": "1,5"
            }
        ]
    },
    "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
    "auto_remove": true
}
```

Response:
```
{
    "initiator": "self",
    "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
    "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
    "credential_offer": {
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "key_correctness_proof": {
            "c": "50760208647425329220085756078365359844108216747857617471675559407347188273783",
            "xz_cap": "875094372301016275109046729353570817972591989578146862827829145207545530848268549800398085359434273457907103601462801753352276189899072787037516367632059131846154491768921681256272407426364093045734768178550674010606540369877673787739826467850854708114905258941128477796617714640482005564322684363695461550388308600220966875802607802118683034549579188350551111843023679944355720029259390028364729547903196971915432315649180753280167668996736946521639437196255582401502228080201768451770345533833655771978307504179105798444224965265358736211032369900002439657322647977176260817804718091386391384239039641553993281064460076408745731001812599323363379316469037000447355870199589197491797282536773",
            "xr_cap": [
                [
                    "student_name",
                    "71887824699095101346517873826078162191438775475710439345915365244207401649653865164227647241453825101698562315000862234732788770786305990360233556293287395751785933928307611029813600385053371368399193329169732460744665311458661268137634166398051271784706442662076604414340585526881895955868730134750804812354773120286690634193582682848806043862121055342807798337090921690573728555918880446552538214795266525420552047368384409029386061165240833253507547930611636998046965758087773047417969105217472843440727911907159097346130577326699543729879602998483516964200895662399275983148814474746252586912813694014509054508896031068863259440477718335230344133236944781346003680306102079874306239847539"
                ],
                [
                    "studiengang",
                    "205962644597913981115342293053023199455692582035615030623594646966083109418524266628720329999224320883294000551741876885048027827221364318588609678359962522549248491691975511235119661593536465929076077836846862435875926708478884346731887233257967946276650822121033881090649109091509636447718681559084233493685706177496526277861240937578330867253627929524229018001729745874013004702052645916467878679435178504833613465155863458915976206900306263781365214728962966847477456872032254120420032548156096668081331705476216266864978059735540087872139320599757421436493369556444105487473588596595388619009828886579445266733457254400744046387870047255896028823985757163864737496056229291406527512541877"
                ],
                [
                    "master_secret",
                    "59840488480943561595691497677808253447288607558044797627175492845610212944487570391479274455388266139030160406495123894241517446051752602412933533894819667145655793699677412484937429765332190420847646855304691244352534154598205092157285179132940783167372749493105560909224565847510034164710488205339947667558352002764154666832912084778769696448669446989514837874951219534614395852507784825766946698884388767922186620099462006557718691049651436348943538243566935125126483397246802497329037238058203669305814044300679115071886416263959219169312526931260971583498902333463790502729375661675723939309610099044842194665666684864545572385362292270638837990135025607690217917878369290438975132012511"
                ],
                [
                    "abschlussnote",
                    "462086574409749563067628158748238622807311008450213839798775954739005621519272654530378945019721202564172493591830448141050179430410402032684377292818897757134143295510723300755448148578010217831196281151463666873536521511543460062159375014394459306615779000620871437157465994887999957348893953577254776751593523692540942867742080713845926694033265498169316895504549734220969563553304158880220344169027871675557076610158968822369129309368036707622960145175956576855632366419963600184315633445371791744779997311427572182554727844878250248984454210694991996939690317976948634083205169049546811336841358688088027909672084359825872851273345923323130833595793188266967493867201658048002192702840529"
                ]
            ]
        },
        "nonce": "516287316320301967436898"
    },
    "auto_offer": false,
    "created_at": "2023-01-24 22:48:14.187784Z",
    "auto_remove": true,
    "credential_proposal_dict": {
        "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential",
        "@id": "931ece2c-5cdb-4eb5-8cad-a65dc9ad0bec",
        "~trace": {
            "target": "log",
            "full_thread": true,
            "trace_reports": []
        },
        "comment": "string",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "credential_proposal": {
            "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
            "attributes": [
                {
                    "name": "student_name",
                    "value": "Max Mustermann"
                },
                {
                    "name": "studiengang",
                    "value": "Informatik"
                },
                {
                    "name": "abschlussnote",
                    "value": "1,5"
                }
            ]
        }
    },
    "trace": true,
    "state": "offer_sent",
    "thread_id": "3e30d792-334a-4d10-9419-ccab0779218e",
    "updated_at": "2023-01-24 22:48:14.187784Z",
    "credential_exchange_id": "0399dfb4-c92d-4235-8f53-88fd3bedd5e4",
    "connection_id": "57057bae-5c9f-42f1-bc26-693db7c41921",
    "auto_issue": true
}
```

**7. Accept Credential Offer (Holder)**

`POST` to `/issue-credential/records`:

Response:
```
{
    "results": [
        {
            "updated_at": "2023-01-24 22:48:18.132074Z",
            "credential_proposal_dict": {
                "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential",
                "@id": "8c1518ab-368a-4109-adfe-ce9f7bc1f39e",
                "credential_proposal": {
                    "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
                    "attributes": [
                        {
                            "name": "student_name",
                            "value": "Max Mustermann"
                        },
                        {
                            "name": "studiengang",
                            "value": "Informatik"
                        },
                        {
                            "name": "abschlussnote",
                            "value": "1,5"
                        }
                    ]
                },
                "comment": "string",
                "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0"
            },
            "credential_exchange_id": "74339329-d8b4-4c46-9ad4-7343d08af7a4",
            "trace": true,
            "auto_remove": true,
            "role": "holder",
            "connection_id": "e2a1eb1e-5756-4930-883f-2915f1540c85",
            "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
            "initiator": "external",
            "auto_issue": false,
            "credential_offer": {
                "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
                "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                "key_correctness_proof": {
                    "c": "50760208647425329220085756078365359844108216747857617471675559407347188273783",
                    "xz_cap": "875094372301016275109046729353570817972591989578146862827829145207545530848268549800398085359434273457907103601462801753352276189899072787037516367632059131846154491768921681256272407426364093045734768178550674010606540369877673787739826467850854708114905258941128477796617714640482005564322684363695461550388308600220966875802607802118683034549579188350551111843023679944355720029259390028364729547903196971915432315649180753280167668996736946521639437196255582401502228080201768451770345533833655771978307504179105798444224965265358736211032369900002439657322647977176260817804718091386391384239039641553993281064460076408745731001812599323363379316469037000447355870199589197491797282536773",
                    "xr_cap": [
                        [
                            "student_name",
                            "71887824699095101346517873826078162191438775475710439345915365244207401649653865164227647241453825101698562315000862234732788770786305990360233556293287395751785933928307611029813600385053371368399193329169732460744665311458661268137634166398051271784706442662076604414340585526881895955868730134750804812354773120286690634193582682848806043862121055342807798337090921690573728555918880446552538214795266525420552047368384409029386061165240833253507547930611636998046965758087773047417969105217472843440727911907159097346130577326699543729879602998483516964200895662399275983148814474746252586912813694014509054508896031068863259440477718335230344133236944781346003680306102079874306239847539"
                        ],
                        [
                            "studiengang",
                            "205962644597913981115342293053023199455692582035615030623594646966083109418524266628720329999224320883294000551741876885048027827221364318588609678359962522549248491691975511235119661593536465929076077836846862435875926708478884346731887233257967946276650822121033881090649109091509636447718681559084233493685706177496526277861240937578330867253627929524229018001729745874013004702052645916467878679435178504833613465155863458915976206900306263781365214728962966847477456872032254120420032548156096668081331705476216266864978059735540087872139320599757421436493369556444105487473588596595388619009828886579445266733457254400744046387870047255896028823985757163864737496056229291406527512541877"
                        ],
                        [
                            "master_secret",
                            "59840488480943561595691497677808253447288607558044797627175492845610212944487570391479274455388266139030160406495123894241517446051752602412933533894819667145655793699677412484937429765332190420847646855304691244352534154598205092157285179132940783167372749493105560909224565847510034164710488205339947667558352002764154666832912084778769696448669446989514837874951219534614395852507784825766946698884388767922186620099462006557718691049651436348943538243566935125126483397246802497329037238058203669305814044300679115071886416263959219169312526931260971583498902333463790502729375661675723939309610099044842194665666684864545572385362292270638837990135025607690217917878369290438975132012511"
                        ],
                        [
                            "abschlussnote",
                            "462086574409749563067628158748238622807311008450213839798775954739005621519272654530378945019721202564172493591830448141050179430410402032684377292818897757134143295510723300755448148578010217831196281151463666873536521511543460062159375014394459306615779000620871437157465994887999957348893953577254776751593523692540942867742080713845926694033265498169316895504549734220969563553304158880220344169027871675557076610158968822369129309368036707622960145175956576855632366419963600184315633445371791744779997311427572182554727844878250248984454210694991996939690317976948634083205169049546811336841358688088027909672084359825872851273345923323130833595793188266967493867201658048002192702840529"
                        ]
                    ]
                },
                "nonce": "516287316320301967436898"
            },
            "thread_id": "3e30d792-334a-4d10-9419-ccab0779218e",
            "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
            "state": "offer_received",
            "auto_offer": false,
            "created_at": "2023-01-24 22:48:18.132074Z"
        }
    ]
}
```
**5. Get Credential Exchange Id (Holder)**

`GET` to `/issue-credential/records`:

Response:
```
{
    "results": [
        {
            "updated_at": "2023-01-24 22:48:18.132074Z",
            "credential_proposal_dict": {
                "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential",
                "@id": "8c1518ab-368a-4109-adfe-ce9f7bc1f39e",
                "credential_proposal": {
                    "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
                    "attributes": [
                        {
                            "name": "student_name",
                            "value": "Max Mustermann"
                        },
                        {
                            "name": "studiengang",
                            "value": "Informatik"
                        },
                        {
                            "name": "abschlussnote",
                            "value": "1,5"
                        }
                    ]
                },
                "comment": "string",
                "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0"
            },
            "credential_exchange_id": "b08af0f9-96fb-4509-9dfb-c25b00fcdf45",
            "trace": true,
            "auto_remove": true,
            "role": "holder",
            "connection_id": "e2a1eb1e-5756-4930-883f-2915f1540c85",
            "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
            "initiator": "external",
            "auto_issue": false,
            "credential_offer": {
                "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
                "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                "key_correctness_proof": {
                    "c": "50760208647425329220085756078365359844108216747857617471675559407347188273783",
                    "xz_cap": "875094372301016275109046729353570817972591989578146862827829145207545530848268549800398085359434273457907103601462801753352276189899072787037516367632059131846154491768921681256272407426364093045734768178550674010606540369877673787739826467850854708114905258941128477796617714640482005564322684363695461550388308600220966875802607802118683034549579188350551111843023679944355720029259390028364729547903196971915432315649180753280167668996736946521639437196255582401502228080201768451770345533833655771978307504179105798444224965265358736211032369900002439657322647977176260817804718091386391384239039641553993281064460076408745731001812599323363379316469037000447355870199589197491797282536773",
                    "xr_cap": [
                        [
                            "student_name",
                            "71887824699095101346517873826078162191438775475710439345915365244207401649653865164227647241453825101698562315000862234732788770786305990360233556293287395751785933928307611029813600385053371368399193329169732460744665311458661268137634166398051271784706442662076604414340585526881895955868730134750804812354773120286690634193582682848806043862121055342807798337090921690573728555918880446552538214795266525420552047368384409029386061165240833253507547930611636998046965758087773047417969105217472843440727911907159097346130577326699543729879602998483516964200895662399275983148814474746252586912813694014509054508896031068863259440477718335230344133236944781346003680306102079874306239847539"
                        ],
                        [
                            "studiengang",
                            "205962644597913981115342293053023199455692582035615030623594646966083109418524266628720329999224320883294000551741876885048027827221364318588609678359962522549248491691975511235119661593536465929076077836846862435875926708478884346731887233257967946276650822121033881090649109091509636447718681559084233493685706177496526277861240937578330867253627929524229018001729745874013004702052645916467878679435178504833613465155863458915976206900306263781365214728962966847477456872032254120420032548156096668081331705476216266864978059735540087872139320599757421436493369556444105487473588596595388619009828886579445266733457254400744046387870047255896028823985757163864737496056229291406527512541877"
                        ],
                        [
                            "master_secret",
                            "59840488480943561595691497677808253447288607558044797627175492845610212944487570391479274455388266139030160406495123894241517446051752602412933533894819667145655793699677412484937429765332190420847646855304691244352534154598205092157285179132940783167372749493105560909224565847510034164710488205339947667558352002764154666832912084778769696448669446989514837874951219534614395852507784825766946698884388767922186620099462006557718691049651436348943538243566935125126483397246802497329037238058203669305814044300679115071886416263959219169312526931260971583498902333463790502729375661675723939309610099044842194665666684864545572385362292270638837990135025607690217917878369290438975132012511"
                        ],
                        [
                            "abschlussnote",
                            "462086574409749563067628158748238622807311008450213839798775954739005621519272654530378945019721202564172493591830448141050179430410402032684377292818897757134143295510723300755448148578010217831196281151463666873536521511543460062159375014394459306615779000620871437157465994887999957348893953577254776751593523692540942867742080713845926694033265498169316895504549734220969563553304158880220344169027871675557076610158968822369129309368036707622960145175956576855632366419963600184315633445371791744779997311427572182554727844878250248984454210694991996939690317976948634083205169049546811336841358688088027909672084359825872851273345923323130833595793188266967493867201658048002192702840529"
                        ]
                    ]
                },
                "nonce": "516287316320301967436898"
            },
            "thread_id": "3e30d792-334a-4d10-9419-ccab0779218e",
            "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
            "state": "offer_received",
            "auto_offer": false,
            "created_at": "2023-01-24 22:48:18.132074Z"
        }
    ]
}
```

**6. Send Request (Holder)**

`POST` to `/issue-credential/records/{cred_ex_id}/send-request` with the "credential_exchange_id" from the previous `GET`-call:

Response:
```
{
    "updated_at": "2023-01-24 22:48:18.132074Z",
    "credential_proposal_dict": {
        "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential",
        "@id": "8c1518ab-368a-4109-adfe-ce9f7bc1f39e",
        "credential_proposal": {
            "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
            "attributes": [
                {
                    "name": "student_name",
                    "value": "Max Mustermann"
                },
                {
                    "name": "studiengang",
                    "value": "Informatik"
                },
                {
                    "name": "abschlussnote",
                    "value": "1,5"
                }
            ]
        },
        "comment": "string",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0"
    },
    "credential_exchange_id": "b08af0f9-96fb-4509-9dfb-c25b00fcdf45",
    "trace": true,
    "auto_remove": true,
    "role": "holder",
    "connection_id": "e2a1eb1e-5756-4930-883f-2915f1540c85",
    "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
    "initiator": "external",
    "auto_issue": false,
    "credential_offer": {
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "key_correctness_proof": {
            "c": "50760208647425329220085756078365359844108216747857617471675559407347188273783",
            "xz_cap": "875094372301016275109046729353570817972591989578146862827829145207545530848268549800398085359434273457907103601462801753352276189899072787037516367632059131846154491768921681256272407426364093045734768178550674010606540369877673787739826467850854708114905258941128477796617714640482005564322684363695461550388308600220966875802607802118683034549579188350551111843023679944355720029259390028364729547903196971915432315649180753280167668996736946521639437196255582401502228080201768451770345533833655771978307504179105798444224965265358736211032369900002439657322647977176260817804718091386391384239039641553993281064460076408745731001812599323363379316469037000447355870199589197491797282536773",
            "xr_cap": [
                [
                    "student_name",
                    "71887824699095101346517873826078162191438775475710439345915365244207401649653865164227647241453825101698562315000862234732788770786305990360233556293287395751785933928307611029813600385053371368399193329169732460744665311458661268137634166398051271784706442662076604414340585526881895955868730134750804812354773120286690634193582682848806043862121055342807798337090921690573728555918880446552538214795266525420552047368384409029386061165240833253507547930611636998046965758087773047417969105217472843440727911907159097346130577326699543729879602998483516964200895662399275983148814474746252586912813694014509054508896031068863259440477718335230344133236944781346003680306102079874306239847539"
                ],
                [
                    "studiengang",
                    "205962644597913981115342293053023199455692582035615030623594646966083109418524266628720329999224320883294000551741876885048027827221364318588609678359962522549248491691975511235119661593536465929076077836846862435875926708478884346731887233257967946276650822121033881090649109091509636447718681559084233493685706177496526277861240937578330867253627929524229018001729745874013004702052645916467878679435178504833613465155863458915976206900306263781365214728962966847477456872032254120420032548156096668081331705476216266864978059735540087872139320599757421436493369556444105487473588596595388619009828886579445266733457254400744046387870047255896028823985757163864737496056229291406527512541877"
                ],
                [
                    "master_secret",
                    "59840488480943561595691497677808253447288607558044797627175492845610212944487570391479274455388266139030160406495123894241517446051752602412933533894819667145655793699677412484937429765332190420847646855304691244352534154598205092157285179132940783167372749493105560909224565847510034164710488205339947667558352002764154666832912084778769696448669446989514837874951219534614395852507784825766946698884388767922186620099462006557718691049651436348943538243566935125126483397246802497329037238058203669305814044300679115071886416263959219169312526931260971583498902333463790502729375661675723939309610099044842194665666684864545572385362292270638837990135025607690217917878369290438975132012511"
                ],
                [
                    "abschlussnote",
                    "462086574409749563067628158748238622807311008450213839798775954739005621519272654530378945019721202564172493591830448141050179430410402032684377292818897757134143295510723300755448148578010217831196281151463666873536521511543460062159375014394459306615779000620871437157465994887999957348893953577254776751593523692540942867742080713845926694033265498169316895504549734220969563553304158880220344169027871675557076610158968822369129309368036707622960145175956576855632366419963600184315633445371791744779997311427572182554727844878250248984454210694991996939690317976948634083205169049546811336841358688088027909672084359825872851273345923323130833595793188266967493867201658048002192702840529"
                ]
            ]
        },
        "nonce": "516287316320301967436898"
    },
    "thread_id": "3e30d792-334a-4d10-9419-ccab0779218e",
    "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
    "state": "offer_received",
    "auto_offer": false,
    "created_at": "2023-01-24 22:48:18.132074Z"
}
```

**7. Store Credential (Holder)**

The credential is automatically issued due to our container configuration. Now it needs to be stored in the local wallet:

`POST` to `/issue-credential/records/{cred_ex_id}/store` with the already known "credential_exchange_id" as parameter and a unique identifier as "credential_id":
```
{
  "credential_id": "string2"
}
```

Response:
```
{
    "updated_at": "2023-01-30 06:13:19.332266Z",
    "credential": {
        "referent": "string2",
        "attrs": {
            "abschlussnote": "1,5",
            "student_name": "Max Mustermann",
            "studiengang": "Informatik für mobile Anwendungen"
        },
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "rev_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
        "cred_rev_id": "1"
    },
    "credential_proposal_dict": {
        "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential",
        "@id": "8aebc0bc-c219-49ac-a8ec-782d50408e9e",
        "credential_proposal": {
            "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
            "attributes": [
            {
                "name": "student_name",
                "value": "Max Mustermann"
            },
            {
                "name": "studiengang",
                "value": "Informatik für mobile Anwendungen"
            },  
            {
                "name": "abschlussnote",
                "value": "1,5"
            }
        ]
        },
        "comment": "string",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0"
    },
    "credential_exchange_id": "7c5c9ec6-1990-450b-bbad-c8f914b686e0",
    "trace": true,
    "auto_remove": true,
    "role": "holder",
    "connection_id": "67d2cd82-8620-47b8-b367-3cc565c92580",
    "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
    "initiator": "external",
    "auto_issue": false,
    "credential_offer": {
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "key_correctness_proof": {
            "c": "50760208647425329220085756078365359844108216747857617471675559407347188273783",
            "xz_cap": "875094372301016275109046729353570817972591989578146862827829145207545530848268549800398085359434273457907103601462801753352276189899072787037516367632059131846154491768921681256272407426364093045734768178550674010606540369877673787739826467850854708114905258941128477796617714640482005564322684363695461550388308600220966875802607802118683034549579188350551111843023679944355720029259390028364729547903196971915432315649180753280167668996736946521639437196255582401502228080201768451770345533833655771978307504179105798444224965265358736211032369900002439657322647977176260817804718091386391384239039641553993281064460076408745731001812599323363379316469037000447355870199589197491797282536773",
            "xr_cap": [
            [
                "student_name",
                "71887824699095101346517873826078162191438775475710439345915365244207401649653865164227647241453825101698562315000862234732788770786305990360233556293287395751785933928307611029813600385053371368399193329169732460744665311458661268137634166398051271784706442662076604414340585526881895955868730134750804812354773120286690634193582682848806043862121055342807798337090921690573728555918880446552538214795266525420552047368384409029386061165240833253507547930611636998046965758087773047417969105217472843440727911907159097346130577326699543729879602998483516964200895662399275983148814474746252586912813694014509054508896031068863259440477718335230344133236944781346003680306102079874306239847539"
            ],   
            [
                "studiengang",
                "205962644597913981115342293053023199455692582035615030623594646966083109418524266628720329999224320883294000551741876885048027827221364318588609678359962522549248491691975511235119661593536465929076077836846862435875926708478884346731887233257967946276650822121033881090649109091509636447718681559084233493685706177496526277861240937578330867253627929524229018001729745874013004702052645916467878679435178504833613465155863458915976206900306263781365214728962966847477456872032254120420032548156096668081331705476216266864978059735540087872139320599757421436493369556444105487473588596595388619009828886579445266733457254400744046387870047255896028823985757163864737496056229291406527512541877"
            ],
            [
                "master_secret",
                "59840488480943561595691497677808253447288607558044797627175492845610212944487570391479274455388266139030160406495123894241517446051752602412933533894819667145655793699677412484937429765332190420847646855304691244352534154598205092157285179132940783167372749493105560909224565847510034164710488205339947667558352002764154666832912084778769696448669446989514837874951219534614395852507784825766946698884388767922186620099462006557718691049651436348943538243566935125126483397246802497329037238058203669305814044300679115071886416263959219169312526931260971583498902333463790502729375661675723939309610099044842194665666684864545572385362292270638837990135025607690217917878369290438975132012511"
            ],
            [
                "abschlussnote",
                "462086574409749563067628158748238622807311008450213839798775954739005621519272654530378945019721202564172493591830448141050179430410402032684377292818897757134143295510723300755448148578010217831196281151463666873536521511543460062159375014394459306615779000620871437157465994887999957348893953577254776751593523692540942867742080713845926694033265498169316895504549734220969563553304158880220344169027871675557076610158968822369129309368036707622960145175956576855632366419963600184315633445371791744779997311427572182554727844878250248984454210694991996939690317976948634083205169049546811336841358688088027909672084359825872851273345923323130833595793188266967493867201658048002192702840529"
            ]
        ]
        },
        "nonce": "83440145324157097344474"
    },
    "credential_request": {
        "prover_did": "BgKt9XKAosTA7fK2JQkPc6",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "blinded_ms": {
            "u": "13017398119098801651203476935906026286595162766955434091046093654004887936166631270604197093082348503648097105528489934792663422540458942802215247930715504603221848206510469937027358752870622763668243027073822213921744062209666348048683999499211491454526470330382640821550168368733254288693106775177521654929521230662199641706905981711145261558030317243767586172869530287851710774849984089490472251766333372754890941043936862251174706425168518652644378243402755526518411607851416233961449732678616507427751445607452111109219917522463844566970659115462860282377020590298458256832234990252618332916079677142105128315885",
            "ur": "1 0CD936F1DAB21B49B37EC501A659BC7A3FACB751D900BD9B2992634B54BBE92E 1 1E9CA1BE802A58008FAC74A23009F27874CAF11AC67DE47BBBAE7EAFA065471E 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8",
            "hidden_attributes": [
                "master_secret"
            ],
            "committed_attributes": {}
           
        },
        "blinded_ms_correctness_proof": {
            "c": "10089775747249386534330606735044991076202540624210532511443643661361103498321",
            "v_dash_cap": "311756666596640414302398874047243399747192602188693056169190763745939234685131487370198991794892119926110733681255920233893820927093367603960884772921917911000137320898233445664286420842130479483273663898857066529975859881494618973852372370844859937947746630065485476860175326847139425184684493717859034729776724072428886731087550606722039710307383272397892316338814481248155233827953295362774773781327564193299758311450153706487489355636631422194773364432050629684722609598151751146775598686331255292396790695970774580540892117299173238791111161618133898586520493165815799082146599496946841581381278612801266202454735082394467829848906302314306999412659512657947211424270420883684489051595139911820212317807248594497",
            "m_caps": {
                "master_secret": "17872116058891238146419296560997839170094883919345514284177010642978101737353911238080135680176210088270121230980625968287367338682388538491806911887478057484816287439009999091638"
            },
            "r_caps": {}
        },
        "nonce": "375853018485121847054574"
    },
    "thread_id": "a7524a07-50cf-4da3-bf9f-44284b6cc15d",
    "credential_request_metadata": {
        "master_secret_blinding_data": {
            "v_prime": "30898275086205917492316318695757047812935273249021371673064419966425650240520748791676434475105336497091549010913087757982133272310449042380384403661305765205610537568455089939419497433386699362116844399894270084901259441775218110566467827301673856477697792167437058701598947863784574478315576015409128001066805757755455048779580106781080762910360733609069519783883492005111362479865892654068359866535298979255199842023302518541481423230951232298136699811708256470516318821552869328088550830327542961804638666399742416201782385259856164014398875532354757103292047705551738227078800047652747929698716620386925025305060918367691474387651567049",
            "vr_prime": "191321F052CCD7CEBE7DBF1FBEEEC8522C1097114FA2D217A7B1507074ECA30A"
        },
        "nonce": "375853018485121847054574",
        "master_secret_name": "default"
    },
    "revoc_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
    "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
    "credential_id": "string2",
    "state": "credential_acked",
    "auto_offer": false,
    "raw_credential": {
        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
        "rev_reg_id": "A9A3zmbBnPT6RcrKvTf9q7:4:A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default:CL_ACCUM:1690937d-d81f-4688-970e-e7eeebef94f1",
        "values": {
            "abschlussnote": {
                "raw": "1,5",
                "encoded": "101797670914524042542252094276754494292099255847339848600323784428616570291319"
            },
            "studiengang": {
                "raw": "Informatik für mobile Anwendungen",
                "encoded": "77985287867295190478109489357414526858109752426758154325670115066810974816434"
            },
            "student_name": {
                "raw": "Max Mustermann",
                "encoded": "100356330819404761362725993527098006389134317767648840535564251586038885819331"
            }
        },
        "signature": {
            "p_credential": {
                "m_2": "79578132337153806277384736633293506363956811049722806960318886851111066310845",
                "a": "76478915558781471790582939311223542956351716887518861036100320581950058644100432660369212827682259185205703079514383738284448600351804778830734610134476512794717971219849463968893768791061845985324701578281488504724132167146762590550195720556064142722843496411099975511162596393750081534953650739954319041076337277553615787925441362478452630273836626298600879699375308110846530075637298086010779974847856111278324916500751841764798075187093757614684149948943445158115739741382662281101887359723505508063959701245133398378544026822769753741577691427809633150239243643765325537564690143335424588403058281263412348422493",
                "e": "259344723055062059907025491480697571938277889515152306249728583105665800713306759149981690559193987143012367913206299323899696942213235956742929683264288949069973560233989833046359",
                "v": "7701143661600367400463664208053523844587426817736771433641074781691885975246373034869749536823083855704533846421223966436938787207844852894470445755890888666866992350249678586470767139581709219553674474317066642559162012327731369122044589651403771778846527716908851948752163549211332104411287140464128734812094601469983527333834363321830449450911633203713112462243528180628748375531577134739008305698323676568992287944832861427428078450365859536725044423944487601188964988660013938559783056202205026882822615155752425326360453216592091880669437113031033046798985764469306440725286676272557945954412042124233624763494154839638657166844639736843614424898292544924160751433861779178640878600587656234683821161490938885343992184666885164543958448727701911877064124433596213099312865274902104588169593934173309536255814900921"
            },
            "r_credential": {
                "sigma": "1 000B7B125277D51C8C74595F6D851E54010E27C0DAD616AFD2969D3988ADBE21 1 099A0806DE3FBDF3D2ADCAF54AEFB0E777DA8DA9778F5BE0322F8E5E8740CEC3 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8",
                "c": "229D4530A3C9994DA03F8595108DF4635945091E56A449B66C2E2A1029FA21C0",
                "vr_prime_prime": "21C3E55DF024BFB9B8A83F14CBF83380094197E411757A7574CC84B047BAD2BA",
                "witness_signature": {
                    "sigma_i": "1 207F8DDF5830B442CD4AFA5106E4086E46DCD9F74179F0F8658C5AFB207D7376 1 1AA1E8051DF313E57D95C6243474B4C8DE49B487169EBEEA08EC8916912B7A55 1 10E6B250DB00F3AA81CE4C5283C5D21C75F8ECE5CB37EFCBCC4EECAE84A48098 1 08EC194C8357DA7E8B3819C88AF4828D3C643F9AFE99FEB89C828717D91DBB53 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8 1 0000000000000000000000000000000000000000000000000000000000000000",
                    "u_i": "1 190ED62857E51AC31762DA3A68989D61F19E76E32247E0AC385BB6EE8A048FAD 1 1E1FC22319AD499951B8E6BFCAFEA3E716B9467FECF42A4A4E128F5B66E5AFC6 1 0ABD941D11EEEADBE4F0BB795FF70519398308AED4A8B94266B635089460EC05 1 16EAEE4BE8642F22DDF691DEC86A51F9FC60ED990A6987F9A8254C764D1877AB 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8 1 0000000000000000000000000000000000000000000000000000000000000000",
                    "g_i": "1 11499D9DB7B16662601341793D8FB3B444B9083E957E9682EC50BB96809336BB 1 089DA741B33E1D8AD1743C53BDD8C36DBF6143BCA58B64FCC7A56D45CBA1EC6E 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8"
                },
                "g_i": "1 11499D9DB7B16662601341793D8FB3B444B9083E957E9682EC50BB96809336BB 1 089DA741B33E1D8AD1743C53BDD8C36DBF6143BCA58B64FCC7A56D45CBA1EC6E 2 095E45DDF417D05FB10933FFC63D474548B7FFFF7888802F07FFFFFF7D07A8A8",
                "i": 1,
                "m2": "AFEFA073407B85FD4FFB11B7E86A608D3FB37064FD357996F0EB95433FFC98BD"
            }
        },
        "signature_correctness_proof": {
            "se": "12008708435687381665687358682161554134748638487130719183421957645332281377810823863311621723831250936942897968924244397154397094076920366791046720498889939777139453286333698028699369765176372144186652357342930443662077750044665032211813794695839600534424002390511933723605483255963734433390015551817341404449252094935991953925420016400130990549142973387775197995913278061125644129116831246349583998885306726562695847290406583739601929982692553903969856895530095802088226608302265338558646904258950017096574704770393793990400985260093808801857649874048248786569314160402025932174685925257888046800079782295541502387342",
            "c": "42212563510861934170033454514212705283947253824151527519031479876706917313409"
        },
        "rev_reg": {
            "accum": "21 12B9CAC10BCF3C7FC77280F46EBAE3461C2B69761B9663113C6B6C232105DE5C2 21 12B3318A1B14D7FB8D11FE2F537763BE50D3959A7D72216FF2330C59480A629A4 6 8A76872F142ADC4CAC87BF09E57AFB29CD9FDCE47A3AD14CC0441D580C531B79 4 19616724B70CCF68F8AB3220F15E41EED41B94DC2BB91DB2DE23FF859A8A2662 6 6C9BD9306E972EB0D97ED4DEEB4D1E7B967871BA283AEF22727CE30746729518 4 2C0E0D7FF0242B674EE94A8C0D9B0E8A1DBC742524E686B8B59352B4D6A2B1F1"
        },
        "witness": {
            "omega": "21 139715F32D58D6DCE15075FD884A666CCF7EE3A418A5FF7197EE0058377A11268 21 12ABD7F9A0B52B280C7C6A381A05D8E5C18A5A9AEEB11751FD6796A6079D0B90F 6 702DFF1086375225FD38CB4E0437BE65C6B51E5E7D0D78D5106C2A45A44F730F 4 25085C417F7F5AD393DA11ECDE7172FEF00F8A30203F6CF50C2FBBCB6ADDD9FC 6 6B3D9667EF9017B30BBFFA159A499FAED73D8ECDF8D87EB41F5B0CB5E6ADE8AF 4 31B0B333AB6ACCEDD97F1A100A8F9CD93313301AD9A951CD0F6B788A3990082A"
        }
    },
    "revocation_id": "1",
    "created_at": "2023-01-30 06:06:14.436659Z"
}
```


### Establish a connection between Verifier and Holder
Same process as establishing connection with *Issuer*.

**1. Create Connection Invitation (Verifier)**

`POST` to `/connections/create-invitation`:

Response:
```
{
    "connection_id": "7d790740-665f-47e0-9809-f1fecc657f8a",
    "invitation": {
        "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation",
        "@id": "f381e588-78d3-4541-80f0-a286bc73b376",
        "label": "verifier.Agent",
        "recipientKeys": [
            "2s8aKGabiMd4s3YqYyMyLmyTRHQN9ytrDjhBrRVQKJVY"
        ],
        "serviceEndpoint": "http://[Server IP]:8070"
    },
    "invitation_url": "http://[Server IP]:8070?c_i=eyJAdHlwZSI6ICJkaWQ6c292OkJ6Q2JzTlloTXJqSGlxWkRUVUFTSGc7c3BlYy9jb25uZWN0aW9ucy8xLjAvaW52aXRhdGlvbiIsICJAaWQiOiAiZjM4MWU1ODgtNzhkMy00NTQxLTgwZjAtYTI4NmJjNzNiMzc2IiwgImxhYmVsIjogInZlcmlmaWVyLkFnZW50IiwgInJlY2lwaWVudEtleXMiOiBbIjJzOGFLR2FiaU1kNHMzWXFZeU15TG15VFJIUU45eXRyRGpoQnJSVlFLSlZZIl0sICJzZXJ2aWNlRW5kcG9pbnQiOiAiaHR0cDovLzM4LjI0Mi4yNDguMjM3OjgwNzAifQ=="
}
```

**2. Accept Connection Invitation (Holder)**

`POST` to `/connections/receive-invitation` with the "invitation" JSON-object from the previous *Verifier*-call as parameter: 
```
{
    "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation",
    "@id": "f381e588-78d3-4541-80f0-a286bc73b376",
    "label": "verifier.Agent",
    "recipientKeys": [
      "2s8aKGabiMd4s3YqYyMyLmyTRHQN9ytrDjhBrRVQKJVY"
    ],
    "serviceEndpoint": "http://[Server IP]:8070"
}
```

Response:
```
{
    "invitation_mode": "once",
    "invitation_key": "2s8aKGabiMd4s3YqYyMyLmyTRHQN9ytrDjhBrRVQKJVY",
    "updated_at": "2023-01-24 23:03:37.769075Z",
    "their_label": "verifier.Agent",
    "request_id": "eceaf982-39c7-4cb2-91d0-edc344429107",
    "routing_state": "none",
    "accept": "auto",
    "state": "request",
    "initiator": "external",
    "connection_id": "4978e5f4-9ece-4555-88cc-776e0e525911",
    "my_did": "BYNDmZ913FoS6Tt94MXxp1",
    "created_at": "2023-01-24 23:03:37.414558Z"
}
```

### Verify a proof
**1. Send Proof Request (Verifier)**

`POST` to `/present-proof/send-request` with... as parameter:
```
{
    "connection_id": "7d790740-665f-47e0-9809-f1fecc657f8a",
    "comment": "string",
    "proof_request": {
        "version": "1.0",
        "name": "Proof request",
        "nonce": "1234567890",
        "requested_attributes": {
            "additionalProp1": {
                "restrictions": [
                    {
                        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                        "schema_name": "bachelorzeugnis_schema",
                        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
                        "issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                        "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                        "schema_issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                        "schema_version": "1.0"
                    }
                ],
                "name": "abschlussnote",
                "non_revoked": {
                    "from_epoch": 1674598033,
                    "to_epoch": 1674598033
                }
            }
        }
    },
    "trace": true
}
```

Response:
```
{
    "presentation_exchange_id": "d1735495-f661-43bd-ae26-86a45690bfeb",
    "auto_present": false,
    "connection_id": "7d790740-665f-47e0-9809-f1fecc657f8a",
    "state": "request_sent",
    "thread_id": "bc275593-a8b0-45ec-8e43-b9d2368bece7",
    "presentation_request": {
        "version": "1.0",
        "name": "Proof request",
        "nonce": "1234567890",
        "requested_attributes": {
            "additionalProp1": {
                "restrictions": [
                    {
                        "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                        "schema_name": "bachelorzeugnis_schema",
                        "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
                        "issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                        "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                        "schema_issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                        "schema_version": "1.0"
                    }
                ],
                "name": "abschlussnote",
                "non_revoked": {
                    "from_epoch": 1674598033,
                    "to_epoch": 1674598033
                }
            }
        }
    },
    "created_at": "2023-01-26 10:34:05.144809Z",
    "role": "verifier",
    "updated_at": "2023-01-26 10:34:05.144809Z",
    "initiator": "self",
    "trace": true
}
```

**2. Get the presentation exchange id (Holder)**

`GET` to `/present-proofs/records`:

Response:
```
{
    "results": [
        {
            "trace": true,
            "thread_id": "bc275593-a8b0-45ec-8e43-b9d2368bece7",
            "presentation_exchange_id": "4e41e57b-2d12-4283-9647-f1bac3c8310e",
            "updated_at": "2023-01-26 10:34:08.990658Z",
            "presentation_request": {
                "version": "1.0",
                "name": "Proof request",
                "nonce": "1234567890",
                "requested_attributes": {
                    "additionalProp1": {
                        "restrictions": [
                            {
                                "cred_def_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                                "schema_name": "bachelorzeugnis_schema",
                                "schema_id": "A9A3zmbBnPT6RcrKvTf9q7:2:bachelorzeugnis_schema:1.0",
                                "issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                                "credential_definition_id": "A9A3zmbBnPT6RcrKvTf9q7:3:CL:10:default",
                                "schema_issuer_did": "Wt1DhMmk4qxiTgmW9QEA8B",
                                "schema_version": "1.0"
                            }
                        ],
                        "name": "abschlussnote",
                        "non_revoked": {
                            "from_epoch": 1674598033,
                            "to_epoch": 1674598033
                        }
                    }
                }
            },
            "role": "prover",
            "state": "request_received",
            "initiator": "external",
            "connection_id": "4978e5f4-9ece-4555-88cc-776e0e525911",
            "created_at": "2023-01-26 10:34:08.990658Z"
        }
    ]
}
```

**3. Present a proof (Holder)**

`POST` to `/present-proof/records/{presentation_exchange_id}/send-presentation` with the "presentation_exchange_id" from the previous `GET`-call:
```
{
    "self_attested_attributes": {},
    "trace": true,
    "requested_attributes": {
        "additionalProp1": {
            "cred_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "revealed": true
        }
    },
    "requested_predicates": { }
}
```

**4. Verify the received proof (Verifier)**

`POST` to `/present-proof/records/{presentation_exchange_id}/verify-presentation` with the "presentation_exchange_id" from the record:

Response:
```
{
    "results": [
        {
            "connection_id": "3a806bdd-82ff-44c1-9cce-23a5c2797d9d",
            "initiator": "self",
            "trace": true,
            "auto_present": false,
            "created_at": "2023-01-31 15:41:48.889996Z",
            "thread_id": "9043c85a-afe5-49ad-ab36-7e269767198e",
            "role": "verifier",
            "presentation_exchange_id": "3c98d2d8-7994-459e-a465-f727dfeb0e9a",
            "updated_at": "2023-01-31 15:47:24.058726Z",
            "verified": "true",
            "presentation": {
                "proof": {
                    "proofs": [],
                    "aggregated_proof": {
                        "c_hash": "10639816310985837783199378615488296256229070620850680453677115602812438341389",
                        "c_list": []
                    }
                },
                "requested_proof": {
                    "revealed_attrs": {},
                        "self_attested_attrs": {
                        "additionalProp1": "1.5"
                },
                "unrevealed_attrs": {},
                "predicates": {}
            },
            "identifiers": []
        },
        "presentation_request": {
            "requested_predicates": {},
            "requested_attributes": {
                "additionalProp1": {
                    "name": "grade"
                }
            },
            "version": "1.0",
            "name": "Proof request",
            "nonce": "1234567890"
        },
        "state": "verified"
    }
    ]
}
```