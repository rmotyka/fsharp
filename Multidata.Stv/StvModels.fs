module Multidata.Stv.StvModels

type Candidate = {
    candidateId: int;
    name: string
}

type VoteItem = {
    candidateId: int;
    preference: int
}

type Vote = VoteItem list

type VoteList = Vote list

type Poll = { 
    numberOfSeats: int;
    candidates: Candidate list
}

type PollResultItem = {
    candidateId: int;
    numberOfVotes: int;
    elected: bool;
}

type PollResult = {
    items: PollResultItem list
}