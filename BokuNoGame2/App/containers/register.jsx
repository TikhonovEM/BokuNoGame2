import React from 'react';
import './css/register.css';

export default class Register extends React.Component {

    render() {
        return (
            <div id="logreg-forms">
                <div className="card rounded-0">
                    <form className="form-signin" asp-action="Register" asp-controller="Account" method="post">
                        <div className="validation" asp-validation-summary="ModelOnly"></div>
                        <h1 className="h3 mb-3 font-weight-normal card-header" style={{ textAlign: "center"}}> Регистрация </h1>

                        <div className="card-body">
                            <input type="text" asp-for="Login" className="form-control" placeholder="Логин" required="" autofocus="" />

                            <input type="password" asp-for="Password" className="form-control" placeholder="Пароль" required="" />

                            <input type="password" asp-for="ConfirmPassword" className="form-control" placeholder="Повторите пароль" required="" />

                            <button className="btn btn-success w-100" type="submit"><i className="fas fa-sign-in-alt"></i> Зарегистрироваться</button>
                        </div>
                    </form>
                </div>
            </div>
        );
    }
}