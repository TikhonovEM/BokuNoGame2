import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';

export default class Game extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            data: {},
            isFetching: true
        };
    }

    componentWillMount() {

        const opts = {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Accept-Encoding': 'gzip;q=1.0, compress;q=0.5'
            }
        };
        fetch("/api/Game/" + this.props.match.params.gameId, opts)
            .then(res => res.json())
            .then((result) => this.setState({
                data: result,
                isFetching: false
            }));
    }

    render() {
        if (this.state.isFetching)
            return <div>...Loading</div>;
        return (
            <div className="container">
                <section>
                    <div>
                        <header className="text-center mb-5">
                            <h1>{this.state.data.game.name}</h1>
                        </header>
                        <div className="main-page row">
                            <div className="image img-fluid col-md-4 mx-auto text-center">
                                <img src="data:image;base64,@System.Convert.ToBase64String(Model.Game.Logo)" style={{ maxHeight: "500px", maxWidth: "400px" }} />
                            </div>
                            <div className="info col-md-4">
                                <h4 className="bg-secondary"><b>Информация:</b></h4>
                                <div>
                                    <b>Жанр: </b><span>@Model.Game.Genre</span>
                                </div>
                                <div>
                                    <b>Разработчик: </b><span>@Model.Game.Developer</span>
                                </div>
                                <div>
                                    <b>Издатель: </b><span>@Model.Game.Publisher</span>
                                </div>
                                <div>
                                    <b>Возрастной рейтинг: </b><span>@Model.Game.AgeRating</span>
                                </div>
                                <div>
                                    <b>Дата выхода: </b><span>@Model.Game.ReleaseDate.ToShortDateString()</span>
                                </div>
                            </div>
                            <div className="rating col-md-4">
                                <h4 className="bg-secondary"><b>Рейтинг:</b></h4>
                                <div className="row">
                                    <div className="col-md-5">
                                        <div className="rateit ml-3 mt-1"
                                            data-rateit-value="@((context.GetGameAverageRating(Model.Game.Id) / 2).ToString(System.Globalization.CultureInfo.InvariantCulture))"
                                            data-rateit-step="0.01"
                                            data-rateit-readonly="true"
                                            data-rateit-mode="font" style={{ fontSize: "40px" }}></div>
                                    </div>
                                    <div className="col-md-7" style={{ fontSize: "35px" }}>@context.GetGameAverageRating(Model.Game.Id).ToString("F2")</div>
                                </div>
                            </div>
                            <div className="description row">
                                <div className="col-md-4">
                                </div>
                                <div className="col-md-8">
                                    <h4 className="bg-secondary"><b>Описание:</b></h4>
                                    <p>@Model.Game.Description.ToHtmlString()</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        );
    }
}