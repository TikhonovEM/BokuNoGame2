import React from 'react'
import { render } from 'react-dom'
import App from './containers/app.jsx'

render(
    <div>
        <header>
            <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-dark border-bottom box-shadow mb-3">
                <div className="container-fluid">
                    <a className="navbar-brand text-light" asp-area="" asp-controller="Home" asp-action="Index" style={{ width: "160px" }}>No Game No Life</a>
                    <button className="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                        <ul className="navbar-nav">
                            <li className="nav-item">
                                <a className="nav-link text-info" asp-area="" asp-controller="Account" asp-action="Register">Регистрация</a>
                            </li>
                            <li className="nav-item">
                                <a className="nav-link text-info" asp-area="" asp-controller="Account" asp-action="Login">Войти</a>
                            </li>
                        </ul>
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
                                <form asp-controller="Game" asp-action="Search" method="post" style={{ marginLeft: "15px" }}>
                                    <button className="btn btn-dark" type="submit">
                                        <i className="fa fa-search"></i> Расширенный поиск
                                            </button>
                                </form>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
        </header>
        <div className="container-fluid">
            <main role="main" className="pb-3">
                <App />
            </main>
        </div>
    </div>,
    document.getElementById('content')
)