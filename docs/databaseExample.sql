CREATE TABLE users
(
    user_id      integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    email        varchar,
    name         varchar,
    visible_name varchar
);

CREATE TABLE user_private_keys
(
    user_private_key_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    user_fk             integer REFERENCES users (user_id),
    keys                varchar
);

CREATE TABLE user_tokens
(
    user_token_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    user_fk       integer REFERENCES users (user_id),
    token         varchar NOT NULL
);

CREATE TABLE global_salt
(
    global_id boolean PRIMARY KEY DEFAULT TRUE,
    pepper    varchar
);

CREATE TABLE passwords
(
    password_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    user_fk     integer REFERENCES users (user_id),
    password    varchar,
    salt        varchar
);

CREATE TABLE totps
(
    totp_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    user_fk integer REFERENCES users (user_id),
    key     varchar
);

CREATE TABLE user_friend
(
    user_fk        integer REFERENCES users (user_id),
    friend_user_fk integer REFERENCES users (user_id),
    status         boolean NOT NULL,
    CONSTRAINT user_friends_id PRIMARY KEY (user_fk, friend_user_fk)
);

CREATE TABLE groups
(
    group_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    name     varchar
);

CREATE TABLE group_user
(
    user_fk       integer REFERENCES users (user_id),
    group_fk      integer REFERENCES groups (group_id),
    administrator boolean NOT NULL,
    CONSTRAINT group_user_id PRIMARY KEY (user_fk, group_fk)
);

CREATE TABLE group_user_public_keys
(
    user_fk    integer REFERENCES users (user_id),
    group_fk   integer REFERENCES groups (group_id),
    public_key varchar NOT NULL,
    CONSTRAINT group_user_public_keys_id PRIMARY KEY (user_fk, group_fk)
);

CREATE TABLE chats
(
    chat_id          integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    name             varchar,
    user1_fk         integer REFERENCES users (user_id),
    user1_public_key varchar NOT NULL,
    user2_fk         integer REFERENCES users (user_id),
    user2_public_key varchar NOT NULL
);

CREATE TABLE group_messages
(
    group_message_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    reference_id     integer,
    group_fk         integer REFERENCES groups (group_id),
    user_fk          integer REFERENCES users (user_id),
    message          varchar,
    timestamp        timestamp
);

CREATE TABLE chat_messages
(
    chat_message_id integer PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    chat_fk         integer REFERENCES chats (chat_id),
    message         varchar,
    timestamp       timestamp
);
