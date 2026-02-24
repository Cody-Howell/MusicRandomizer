CREATE TABLE web_version(
    v int NOT NULL
);

INSERT INTO web_version VALUES (1);

CREATE TABLE audio(
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    userGivenName varchar(200) NOT NULL, 
    author varchar(200) NOT NULL,
    audio bytea NOT NULL
);

CREATE TABLE web_info(
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    webName varchar(200) NOT NULL, 
    info varchar(200) NOT NULL,
    authorRoute varchar(500) NOT NULL
);

CREATE TABLE web_nodes(
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    title varchar(50) NOT NULL, 
    artist varchar(100) NOT NULL, 
    audioId int references audio(id) NOT NULL,
    lengthToNext int NOT NULL, 
    start real NOT NULL
);

CREATE TABLE node_targets(
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    target varchar(50) references web_nodes(title) NOT NULL, 
    index int NULL, 
    weight json NOT NULL
);