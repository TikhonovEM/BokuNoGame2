import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch, NavLink } from 'react-router-dom';

export default class GameList extends React.Component {
    render() {
        return (
            <div>
                <NavLink to='/Game/4'>Нажми меня</NavLink>
            </div>
            );
    }
}