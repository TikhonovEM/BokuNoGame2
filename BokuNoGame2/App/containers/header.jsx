import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch, NavLink } from 'react-router-dom';

export default class Header extends React.Component {
    render() {
        const userInfo = JSON.parse(localStorage.getItem("userInfo"));
        return (
            <header>
                <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-dark border-bottom box-shadow mb-3">
                    <div className="container-fluid">
                        <NavLink to='/' className="navbar-brand text-light" style={{ width: "160px" }}>No Game No Life</NavLink>
                        <button className="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                            aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                            {
                                (userInfo != null && userInfo.isSignedIn)
                                    ? <ul className="navbar-nav">
                                        {
                                            userInfo.roles.includes("Admin") &&
                                            <li className="nav-item">
                                                <NavLink to="/" className="nav-link text-info">Администрирование</NavLink>
                                            </li>
                                        }
                                        <li className="nav-item">
                                            <NavLink to='/Account/Profile' className="nav-link text-info">Профиль</NavLink>
                                        </li>
                                        <li className="nav-item">
                                            <button type="button" className="nav-link btn btn-link text-info" onClick={e => logout()}>Выйти</button>
                                        </li>
                                    </ul>
                                    : <ul className="navbar-nav">
                                        <li className="nav-item">
                                            <NavLink to='/Account/Register' className="nav-link text-info">Регистрация</NavLink>
                                        </li>
                                        <li className="nav-item">
                                            <NavLink to='/Account/Login' className="nav-link text-info">Войти</NavLink>
                                        </li>
                                    </ul>
                            }
                            <ul className="navbar-nav flex-grow-1">
                                <li className="nav-item">
                                    <form className="form-inline" asp-controller="Game" asp-action="GameByName" method="post">
                                        <div className="input-group">
                                            <label htmlFor="likeName"></label>
                                            <input type="search" id="likeName" name="likeName" className="form-control-sm" placeholder="Введите название игры" size="100" />
                                            <button className="btn btn-outline-secondary" type="submit">
                                                <i className="fa fa-search"></i>
                                            </button>
                                        </div>
                                    </form>
                                </li>
                                <li className="nav-item">
                                    <NavLink to='/GameList' style={{ marginLeft: "15px" }}>
                                        <button className="btn btn-dark" type="submit">
                                            <i className="fa fa-search"></i> Расширенный поиск
                                        </button>
                                    </NavLink>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>
        );
    }
};