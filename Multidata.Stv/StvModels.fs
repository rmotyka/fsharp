module Multidata.Stv.StvModels

type Candidate = {
    candidateId: int;
    name: string
}


type Ballot = int list // candidate in order of preference

type Poll = { 
    numberOfSeats: int;
    candidates: Candidate list
}

type PollResultItem = {
    candidateId: int;
    numberOfVotes: int;
    elected: bool;
}

// --- temporary types

type AggregatedVote = {ballot: Ballot; numberOfVotes: int}
