--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.15
-- Dumped by pg_dump version 9.6.15

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- Name: attendance_pk_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.attendance_pk_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.attendance_pk_id_seq OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: attendance; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.attendance (
    pk_id bigint DEFAULT nextval('public.attendance_pk_id_seq'::regclass) NOT NULL,
    is_present boolean,
    entry_datetime timestamp without time zone,
    exit_datetime timestamp without time zone,
    roll_no bigint
);


ALTER TABLE public.attendance OWNER TO postgres;

--
-- Name: centre; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.centre (
    centre_id integer NOT NULL,
    centre_name character varying(255)
);


ALTER TABLE public.centre OWNER TO postgres;

--
-- Name: centre_centre_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.centre_centre_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.centre_centre_id_seq OWNER TO postgres;

--
-- Name: centre_centre_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.centre_centre_id_seq OWNED BY public.centre.centre_id;


--
-- Name: seq_desc_id; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.seq_desc_id
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.seq_desc_id OWNER TO postgres;

--
-- Name: description; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.description (
    id integer DEFAULT nextval('public.seq_desc_id'::regclass) NOT NULL,
    image character varying,
    roll_no bigint,
    description double precision[]
);


ALTER TABLE public.description OWNER TO postgres;

--
-- Name: seq_id; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.seq_id
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.seq_id OWNER TO postgres;

--
-- Name: students; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.students (
    roll_no bigint NOT NULL,
    college character varying(255),
    course character varying(255),
    dob timestamp without time zone,
    email character varying(255),
    mobile_no character varying(255),
    student_name character varying(255),
    centre_id integer
);


ALTER TABLE public.students OWNER TO postgres;

--
-- Name: students_roll_no_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.students_roll_no_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.students_roll_no_seq OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id character varying(255) NOT NULL,
    admin_password character varying(255),
    create_date timestamp without time zone,
    full_name character varying(255),
    is_valid boolean,
    user_role character varying(255),
    centre_id integer
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: centre centre_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.centre ALTER COLUMN centre_id SET DEFAULT nextval('public.centre_centre_id_seq'::regclass);


--
-- Data for Name: attendance; Type: TABLE DATA; Schema: public; Owner: postgres
--


--
-- Name: attendance_pk_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.attendance_pk_id_seq', 34, true);


--
-- Data for Name: centre; Type: TABLE DATA; Schema: public; Owner: postgres
--

--
-- Name: centre_centre_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.centre_centre_id_seq', 1, false);


--
-- Data for Name: description; Type: TABLE DATA; Schema: public; Owner: postgres
--

--
-- Name: seq_desc_id; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.seq_desc_id', 3382, true);


--
-- Name: seq_id; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.seq_id', 1, false);


--
-- Data for Name: students; Type: TABLE DATA; Schema: public; Owner: postgres
--

--
-- Name: students_roll_no_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.students_roll_no_seq', 1, false);


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--


--
-- Name: attendance attendance_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_pkey PRIMARY KEY (pk_id);


--
-- Name: centre centre_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.centre
    ADD CONSTRAINT centre_pkey PRIMARY KEY (centre_id);


--
-- Name: description pk_desc_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.description
    ADD CONSTRAINT pk_desc_id PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- Name: users fkiuvd58iwpdgdmdmofk5lwf8e1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fkiuvd58iwpdgdmdmofk5lwf8e1 FOREIGN KEY (centre_id) REFERENCES public.centre(centre_id);


--
-- PostgreSQL database dump complete
--

