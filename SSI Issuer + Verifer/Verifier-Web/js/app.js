const verifierAddress = 'http://38.242.248.237:8071';
var connectionPollingTimer;

const loadPage = async () => {
    await loadConnections();
    await loadSchemas();
    await loadCredentialDefinitions();
    connectionPollingTimer = setInterval(loadConnections, 10000);
}

const loadConnections = async () => {
    const response = await fetch(verifierAddress + '/connections');
    const json = await response.json();
    const results = json.results;
    console.log("Loaded Connections:\n");
    console.log(results);
    const selectElement = document.getElementById("connections")
    selectElement.innerHTML = '';
    for(let i = 0; i < results.length; i++){
        if(results[i].state === 'active' || results[i].state === 'response'){
            let option = document.createElement('option');
            option.value = results[i].connection_id;
            option.textContent = results[i].their_label + ' (' + results[i].connection_id + ')';
            selectElement.appendChild(option);
        }
    }
}

const createQRCode = async () => {
    let qrCode = new QRCode(document.getElementById('connection_qr'), {
        width: 200,
        height: 200,
        colorDark : "#000000",
        colorLight : "#ffffff",
        correctLevel : QRCode.CorrectLevel.H
    });
    qrCode.clear();
    const response = await fetch(verifierAddress + '/connections/create-invitation', {
        method: 'POST',
        body: '',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    const responseJson = await response.json();
    console.log(JSON.stringify(responseJson.invitation));
    qrCode.makeCode('{' + JSON.stringify(responseJson.invitation) + '}');
}

const loadCredentialDefinitions = async () => {
    const response = await fetch(verifierAddress + '/credential-definitions/created');
    const json = await response.json();
    const credDefIds = json.credential_definition_ids;
    console.log("Load credential definitions:");
    for(let i = 0; i < credDefIds.length; i++){
        let credDef = await loadCredentialDefinition(credDefIds[i]);
        console.log(credDef);
    }
}

const loadCredentialDefinition = async (id) => {
    const response = await fetch(verifierAddress + '/credential-definitions/' + id);
    const json = await response.json();
    return json.credential_definition;
}

const loadCredentialDefinitionForSchema = async(schemaId) => {
    /*
    const response = await fetch(verifierAddress + '/credential-definitions/created?schema_id=' + schemaId);
    const json = await response.json();
    return json.credential_definition_ids;
    */
    if(schemaId == 'A9A3zmbBnPT6RcrKvTf9q7:2:Masterabschluss:1.0'){
        return ['A9A3zmbBnPT6RcrKvTf9q7:3:CL:22:default'];
    } else if(schemaId == 'A9A3zmbBnPT6RcrKvTf9q7:2:Bachelorabschluss:1.0') {
        return  ['A9A3zmbBnPT6RcrKvTf9q7:3:CL:20:default'];
    }
}

const loadSchemas = async () => {
    const credDefSelect = document.getElementById("credDefs");
    const schemaIds = ['A9A3zmbBnPT6RcrKvTf9q7:2:Masterabschluss:1.0', 'A9A3zmbBnPT6RcrKvTf9q7:2:Bachelorabschluss:1.0'];
    console.log("Load schemas:");
    credDefSelect.innerHTML = '';
    for(let i = 0; i < schemaIds.length; i++){
        let schema = await loadSchema(schemaIds[i]);
        console.log(schema);

        let credDefIds = await loadCredentialDefinitionForSchema(schema.id)

        let option = document.createElement("option");
        option.value = credDefIds[0];
        option.textContent = schema.name + " ("+ credDefIds[0] +")";
        credDefSelect.appendChild(option);
    }
}

const loadSchema = async (id) => {
    const response = await fetch(verifierAddress + '/schemas/' + id);
    const json = await response.json();
    return json.schema;
}

const sendProofRequest = async () => {
    const connection = document.getElementById("connections").value;
    const attribute = document.getElementById('proof-attributes').value;
    const jsonBody = '{"connection_id": "' + connection + '", "proof_request": { "requested_predicates": {}, "requested_attributes": { "additionalProp1": { "name": "' + attribute +  '" } }, "version": "1.0", "name": "Proof request", "nonce": "1234567890" }, "trace": true, "comment": "string"}';

    console.log("Send proof request to: " + connection);
    console.log(jsonBody);

    const response = await fetch(verifierAddress + '/issue-credential/send-offer', {
        method: 'POST',
        body: jsonBody,
        headers: {
            'Content-Type': 'application/json'
        }
    })
    const responseJson = await response.json();
    console.log("Response:\n{" + JSON.stringify(responseJson) + "}");
}