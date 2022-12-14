import React, { useEffect, useState } from 'react';
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';
import { Nav, Navbar, NavbarBrand, NavItem, NavLink, Form, FormGroup } from 'reactstrap'
import { Link, Outlet } from 'react-router-dom';

import { useRecoilValue } from 'recoil';
import { authAtom } from '../_state';
import { useUserActions } from '../_actions';
import { history } from '../_helpers/history';

export { Login };

function Login() {
    const auth = useRecoilValue(authAtom);
    const userActions = useUserActions();
    const [activeTab, setActiveTab] = useState(0);

    useEffect(() => {
        // redirect to home if already logged in
        if (auth) {
            history.push('/');
            history.go('/');
        }

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [auth]);

    // form validation rules 
    const validationSchema = Yup.object().shape({
        password: Yup.string().required('password is required'),
        id: Yup.string().required('User Id is required')
    });
    const formOptions = { resolver: yupResolver(validationSchema) };

    // get functions to build form with useForm() hook
    const { register, handleSubmit, setError, formState } = useForm(formOptions);
    const { errors, isSubmitting } = formState;

    function onSubmit({ username, password, id }) {
        return userActions.login(username, password, id, activeTab)
            .catch(error => {
                setError('apiError', { message: error });
            });
    }

    return (
        <div>
            <header>
                <Navbar className="navbar navbar-expand navbar-dark bg-dark" container light>
                    <NavbarBrand tag={Link} to="/">Contract Portal</NavbarBrand>
                </Navbar>
            </header>
            <div className="col-md-6 offset-md-3 mt-5">
                <div className="card">
                    <Nav tabs justified>
                        <NavItem>
                            <NavLink
                                className={activeTab === 0 ? 'active' : ''}
                                onClick={function noRefCheck() { setActiveTab(0); }}
                            >
                                Sign In
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                className={activeTab === 1 ? 'active' : ''}
                                onClick={function noRefCheck() { setActiveTab(1); }}
                            >
                                Register
                            </NavLink>
                        </NavItem>
                    </Nav>
                    <div className="card-body">
                        <Form onSubmit={handleSubmit(onSubmit)}>
                            {
                                activeTab === 1 ?
                                    <FormGroup className="form-group">
                                        <label>Username</label>
                                        <input name="username" type="text" {...register('username')} className={`form-control ${errors.username ? 'is-invalid' : ''}`} />
                                        <div className="invalid-feedback">{errors.username?.message}</div>
                                    </FormGroup> :
                                    <></>
                            }
                            <FormGroup className="form-group">
                                <label>User ID</label>
                                <input name="id" type="text" {...register('id')} className={`form-control ${errors.id ? 'is-invalid' : ''}`} />
                                <div className="invalid-feedback">{errors.id?.message}</div>
                            </FormGroup>
                            <FormGroup className="form-group">
                                <label>Password</label>
                                <input name="password" type="password" {...register('password')} className={`form-control ${errors.password ? 'is-invalid' : ''}`} />
                                <div className="invalid-feedback">{errors.password?.message}</div>
                            </FormGroup>
                            <button disabled={isSubmitting} className="btn btn-primary">
                                {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                                {activeTab === 0 ? 'Sign In' : 'Register'}
                            </button>
                            {errors.apiError &&
                                <div className="alert alert-danger mt-3 mb-0">{errors.apiError?.message}</div>
                            }
                        </Form>
                    </div>
                </div>
            </div>
        </div>
    )
}
