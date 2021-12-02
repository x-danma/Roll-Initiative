import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiPaths } from 'src/environments/environment';

@Component({
    selector: 'app-game',
    templateUrl: './game.component.html',
    styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
    public game: Game = {id:0, isCompleted:false,characters:[]};
    public showCharacterList = false;
    constructor(
        private activatedRoute: ActivatedRoute,
        http: HttpClient,
    ) {
        this.activatedRoute = activatedRoute;
        let gameid = this.activatedRoute.snapshot.params.id;
        http.get<Game>(ApiPaths.ApiBaseUrl + 'games/' + gameid).subscribe(result => {
            this.game = result;
            this.showCharacterList = true;
        }, error => console.error(error));
    }

    ngOnInit(): void {


    }

}


interface Game {
    id: number;
    isCompleted: boolean;
    characters: Character[];
}

interface Character {
    name: string;
    initiativeRoll: number;
}