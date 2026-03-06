-- =============================================
-- Busca Promoção - Script de criação do banco
-- PostgreSQL
-- =============================================

CREATE TABLE IF NOT EXISTS usuario (
    id              UUID PRIMARY KEY,
    nome            VARCHAR(150) NOT NULL,
    email           VARCHAR(150) NOT NULL UNIQUE,
    senha_hash      TEXT,
    criado_em       TIMESTAMP NOT NULL DEFAULT NOW(),
    atualizado_em   TIMESTAMP
);

CREATE TABLE IF NOT EXISTS produto (
    id              UUID PRIMARY KEY,
    nome            VARCHAR(100) NOT NULL,
    usuario_id      UUID NOT NULL REFERENCES usuario(id) ON DELETE CASCADE,
    criado_em       TIMESTAMP NOT NULL DEFAULT NOW(),
    atualizado_em   TIMESTAMP
);

CREATE TABLE IF NOT EXISTS perfil (
    id              UUID PRIMARY KEY,
    handle_perfil   VARCHAR(50) NOT NULL,
    usuario_id      UUID NOT NULL REFERENCES usuario(id) ON DELETE CASCADE,
    criado_em       TIMESTAMP NOT NULL DEFAULT NOW(),
    atualizado_em   TIMESTAMP
);

CREATE TABLE IF NOT EXISTS notificacao (
    id              UUID PRIMARY KEY,
    titulo          VARCHAR(200) NOT NULL,
    conteudo        VARCHAR(1000) NOT NULL,
    url_tweet       VARCHAR(500),
    handle_perfil   TEXT,
    postado_em      TIMESTAMP,
    foi_lida        BOOLEAN NOT NULL DEFAULT FALSE,
    usuario_id      UUID NOT NULL REFERENCES usuario(id) ON DELETE CASCADE,
    criado_em       TIMESTAMP NOT NULL DEFAULT NOW(),
    atualizado_em   TIMESTAMP
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_produto_usuario_id ON produto(usuario_id);
CREATE INDEX IF NOT EXISTS idx_perfil_usuario_id ON perfil(usuario_id);
CREATE INDEX IF NOT EXISTS idx_notificacao_usuario_id ON notificacao(usuario_id);
CREATE INDEX IF NOT EXISTS idx_notificacao_foi_lida ON notificacao(usuario_id, foi_lida);
