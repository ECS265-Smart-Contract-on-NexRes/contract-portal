import React, { Component, useEffect } from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import './custom.css';
import { useRecoilValue } from 'recoil';
import { Login } from './components/Login';
import { authAtom } from './_state';
import { NavMenu } from './components/NavMenu'
import { useSetRecoilState } from 'recoil';
import { userBalanceAtom } from './_state';
import { useFetchWrapper } from './_helpers';

function App() {
  const auth = useRecoilValue(authAtom);
  const setUserBalance = useSetRecoilState(userBalanceAtom);
  const fetchWrapper = useFetchWrapper();

  function loadBalance() {
    if (auth) {
      fetchWrapper.get('api/balance/get')
        .then((res) => {
          setUserBalance(res);
        });
    }
  }

  useEffect(loadBalance, [])

  return (
    <div>
      <Routes>
        <Route path='/login' element={<Login />} />
        {auth ?
          <Route path='/' element={<NavMenu />}>
            {AppRoutes.map((route, index) => {
              const { element, ...rest } = route;
              return <Route key={index} {...rest} element={element} />;
            })}
          </Route> :
          <Route path="*" element={<Navigate to='/login' />} />}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </div>
  );

}

export default App;