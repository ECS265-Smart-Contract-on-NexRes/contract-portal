import React, { Component } from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import './custom.css';
import { useRecoilValue } from 'recoil';
import { Login } from './components/Login';
import { authAtom } from './_state';
import {NavMenu} from './components/NavMenu'

function App() {
  const auth = useRecoilValue(authAtom);

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