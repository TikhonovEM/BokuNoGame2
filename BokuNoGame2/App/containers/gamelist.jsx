import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch, NavLink } from 'react-router-dom';
import './css/gamelist.css'

export default class GameList extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            data: {},
            isFetching: true
        };
        this.getPage = this.getPage.bind(this);
    }

    getPage(page) {
        this.setState({
            isFetching: true
        })
        const opts = {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Accept-Encoding': 'gzip;q=1.0, compress;q=0.5'
            }
        };
        fetch(`/api/Game/GameList?page=${page}`, opts)
            .then(res => res.json())
            .then((result) => this.setState({
                data: result,
                isFetching: false
            }));
    }

    componentDidMount() {
        this.getPage(1);
    }

    render() {
        if (this.state.isFetching)
            return <div>...Loading</div>;
        var pageNumber = this.state.data.pagination.pageNumber;
        return (
            <div className="row">
                <div className="col-md-8 order-md-1">
                    <div className="row">
                        <ul className="list-group list-group-horizontal repeat">
                            {this.state.data.games.map(function (value, index, array) {
                                return (
                                    <li className="list-group-item btn btn-outline-light" key={index}>
                                        <NavLink to={'/Game/' + value.id}>
                                            <div>
                                                <img src={"data:image;base64," + value.logo} width="80" height="80" />
                                                <div>{value.name}</div>
                                            </div>
                                        </NavLink>
                                    </li>
                                );
                            })
                            }
                        </ul>
                    </div>
                    {this.state.data.pagination.hasPreviousPage &&
                        <button className="btn btn-outline-dark" onClick={this.getPage.bind(this, pageNumber - 1)}>
                            <i className="glyphicon glyphicon-chevron-left"></i>Назад
                        </button>
                    }
                    {this.state.data.pagination.hasNextPage &&
                        <button className="btn btn-outline-dark" onClick={this.getPage.bind(this, pageNumber + 1)}>
                            Вперед<i className="glyphicon glyphicon-chevron-right"></i>
                        </button>
                    }
                </div>
            </div>
        );
    }
}