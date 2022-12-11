import React, { Component, useEffect, useState } from 'react';
import { Button, Container } from 'reactstrap';
import OperationModal from './OperationModal';
import { useFetchWrapper } from '../_helpers/fetchWrapper';
import { RevolvingDot } from 'react-loader-spinner';
import { FcOk } from 'react-icons/fc';

export function Home() {
  const displayName = Home.name;
  const fetchWrapper = useFetchWrapper();
  const [kvStatus, setKvStatus] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedKey, setSelectedKey] = useState(null);

  useEffect(() => {
    populateContractData();
  }, [])

  const populateContractData = async function () {
    const response = await fetchWrapper.get('api/binary/list');
    const data = response;
    setKvStatus(data);
    setLoading(false);
  }

  const undoSelect = function () {
    setSelectedKey(null);
  }

  const renderBinariesTable = function (kvStatus) {
    return (
      <div>
        {kvStatus.map((item) =>
          <OperationModal key={item.key} selectedKey={selectedKey} undo={undoSelect.bind(this)} modal={false} kvStatus={item}></OperationModal>
        )}
        <table className='table table-striped' aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>File Name</th>
              <th>Key</th>
              <th style={{textAlign: "center"}}>Status</th>
            </tr>
          </thead>
          <tbody>
            {kvStatus.map((item) =>
              <tr
                onClick={() => { setSelectedKey(item.key) }}
                key={item.key}>
                <td>{item.name}</td>
                <td>{item.key}</td>
                <td style={{textAlign: "center"}}>{item.isPublished ?  <FcOk /> : <RevolvingDot
                  height="30"
                  width="30"
                  color="#4fa94d"
                  secondaryColor=''
                  ariaLabel="revolving-dot-loading"
                  radius="5"
                  wrapperStyle={{}}
                  wrapperClass=""
                  visible={true}
                />}</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    );
  }


  return (
    <Container>
      <h1 id="tabelLabel" >Smart Contracts</h1>
      {loading
        ? <p><em>Loading...</em></p>
        : renderBinariesTable(kvStatus)}
    </Container>
  );
}
