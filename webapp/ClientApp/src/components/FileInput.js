import React, { useState } from 'react';
import {
    Form,
    FormGroup,
    Label,
    Input,
    Button,
    Alert
} from 'reactstrap';
import { useFetchWrapper } from '../_helpers/fetchWrapper';

export const FileInput = () => {
    const [name, setName] = useState('');
    const [body, setBody] = useState('');
    const [showSuccessAlert, setSuccessAlert] = useState(false);
    const [showFailureAlert, setFailureAlert] = useState(false);
    const fetchWrapper = useFetchWrapper();

    const handleSubmit = async (e) => {
        e.preventDefault();

        const data = new FormData();
        data.append('body', body);
        data.append('name', name);

        fetchWrapper.post('api/binary/upload', null, {
            method: 'POST',
            headers: {
                'enctype': 'multipart/form-data'
            },
            body: data
        })
            .then((res) => {
                console.log(`binary named '${name}' uploaded successfully`);
                setSuccessAlert(true);
            }).catch((e) => {
                console.log(`binary upload failed: ${e}`);
                setFailureAlert(true);
            });
    }

    const onSuccessAlertDismiss = () => setSuccessAlert(false);
    const onFailureAlertDismiss = () => setFailureAlert(false);

    return (
        <div>
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Input
                        type="file"
                        onChange={(e) => {
                            setSuccessAlert(false);
                            setFailureAlert(false);
                            setBody(e.target.files[0]);
                            setName(e.target.files[0].name);
                        }}
                    />
                </FormGroup>
                <FormGroup>
                    <Label for="binaryName">
                        Contract Name
                    </Label>
                    <Input
                        value={name}
                        onChange={(e) => {
                            setName(e.target.value);
                        }}
                    />
                </FormGroup>
                <Button type="submit">Submit</Button>
            </Form>
            <br></br>
            <Alert color="info" isOpen={showSuccessAlert} toggle={onSuccessAlertDismiss}>
                Smart contract <i>{name}</i> uploaded successfully
            </Alert>
            <Alert color="danger" isOpen={showFailureAlert} toggle={onFailureAlertDismiss}>
                Failed to upload the binary file!
            </Alert>
        </div>
    );
}
