import React, { useEffect } from 'react';
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';
import {Nav, NavItem, NavLink} from 'reactstrap'

import { useRecoilValue } from 'recoil';
import { authAtom } from '../_state';
import { useUserActions } from '../_actions';
import { history } from '../_helpers/history';

export { Login };

function Login() {
    const auth = useRecoilValue(authAtom);
    const userActions = useUserActions();

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
        username: Yup.string().required('Username is required'),
        password: Yup.string().required('Password is required')
    });
    const formOptions = { resolver: yupResolver(validationSchema) };

    // get functions to build form with useForm() hook
    const { register, handleSubmit, setError, formState } = useForm(formOptions);
    const { errors, isSubmitting } = formState;

    function onSubmit({ username, password }) {
        return userActions.login(username, password)
            .catch(error => {
                setError('apiError', { message: error });
            });
    }

    return (
        <div className="col-md-6 offset-md-3 mt-5">
            <div className="card">
                <Nav>
                    <NavItem>
                        <NavLink
                            active
                            href="#"
                        >
                            Link
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink href="#">
                            Another Link
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink
                            disabled
                            href="#"
                        >
                            Disabled Link
                        </NavLink>
                    </NavItem>
                </Nav>
                <div className="card-body">
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className="form-group">
                            <label>Username</label>
                            <input name="username" type="text" {...register('username')} className={`form-control ${errors.username ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.username?.message}</div>
                        </div>
                        <div className="form-group">
                            <label>Password</label>
                            <input name="password" type="password" {...register('password')} className={`form-control ${errors.password ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.password?.message}</div>
                        </div>
                        <button disabled={isSubmitting} className="btn btn-primary">
                            {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                            Login
                        </button>
                        {errors.apiError &&
                            <div className="alert alert-danger mt-3 mb-0">{errors.apiError?.message}</div>
                        }
                    </form>
                </div>
            </div>
        </div>
    )
}
