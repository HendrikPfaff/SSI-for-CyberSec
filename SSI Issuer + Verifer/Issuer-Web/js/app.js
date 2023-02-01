const issuerAddress = 'http://38.242.248.237:8051';
var connectionPollingTimer;

const loadPage = async () => {
    await loadConnections();
    await loadSchemas();
    await loadCredentialDefinitions();
    connectionPollingTimer = setInterval(loadConnections, 10000);
}

const loadConnections = async () => {
    const response = await fetch(issuerAddress + '/connections');
    const json = await response.json();
    const results = json.results;
    console.log("Load active Connections:\n");
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
    const response = await fetch(issuerAddress + '/connections/create-invitation', {
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
    const response = await fetch(issuerAddress + '/credential-definitions/created');
    const json = await response.json();
    const credDefIds = json.credential_definition_ids;
    console.log("Load credential definitions:");
    for(let i = 0; i < credDefIds.length; i++){
        let credDef = await loadCredentialDefinition(credDefIds[i]);
        console.log(credDef);
    }
}

const loadCredentialDefinition = async (id) => {
    const response = await fetch(issuerAddress + '/credential-definitions/' + id);
    const json = await response.json();
    return json.credential_definition;
}

const loadCredentialDefinitionForSchema = async(schemaId) => {
    const response = await fetch(issuerAddress + '/credential-definitions/created?schema_id=' + schemaId);
    const json = await response.json();
    return json.credential_definition_ids;
}

const loadSchemas = async () => {
    const credDefSelect = document.getElementById("credDefs");

    const response = await fetch(issuerAddress + '/schemas/created');
    const json = await response.json();
    const schemaIds = json.schema_ids;
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
    const response = await fetch(issuerAddress + '/schemas/' + id);
    const json = await response.json();
    return json.schema;
}

const sendCredential = async () => {
    // Build the json body.
    const connection = document.getElementById("connections").value;
    const credDef = document.getElementById("credDefs").value;
    const studentName = document.getElementById("student_name").value;
    const programme = document.getElementById("programme").value;
    const grade = document.getElementById("grade").value;
    console.log("Send Credential for " +  credDef);

    const jsonBody = '{"connection_id": "' + connection + '", "auto_issue": true, "trace": true, "comment": "string", "credential_preview": { "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview", "attributes": [{"name": "student_name", "value": "' + studentName + '"}, {"name": "programme", "value": "' + programme +'"}, {"name": "grade", "value": "' + grade + '"}]}, "cred_def_id": "' + credDef + '", "auto_remove": true}';
    console.log(jsonBody);

    // Send the request.
    const response = await fetch(issuerAddress + '/issue-credential/send-offer', {
        method: 'POST',
        body: jsonBody,
        headers: {
            'Content-Type': 'application/json'
        }
    })
    const responseJson = await response.json();
    console.log("Response:\n{" + JSON.stringify(responseJson) + "}");

    studentName.value = "";
    programme.value = "";
    grade.value = "";
}