CREATE DATABASE sistema_estoque;
USE sistema_estoque;

CREATE TABLE categoria (
    id_categoria INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255)
);

CREATE TABLE fornecedor (
    id_fornecedor INT IDENTITY(1,1) PRIMARY KEY,
    razao_social VARCHAR(150) NOT NULL,
    nome_fantasia VARCHAR(150),
    cnpj VARCHAR(18) UNIQUE,
    telefone VARCHAR(20),
    email VARCHAR(100),
    endereco VARCHAR(255)
);

CREATE TABLE cliente (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    cpf VARCHAR(14) UNIQUE,
    telefone VARCHAR(20),
    email VARCHAR(100),
    endereco VARCHAR(255)
);

CREATE TABLE usuario (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    login VARCHAR(50) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL
);

CREATE TABLE produto (
    id_produto INT IDENTITY(1,1) PRIMARY KEY,
    id_categoria INT NOT NULL,
    id_fornecedor INT NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(255),
    codigo_barras VARCHAR(50) UNIQUE,
    preco_custo DECIMAL(10,2),
    preco_venda DECIMAL(10,2),
    estoque INT DEFAULT 0,
    FOREIGN KEY (id_categoria) REFERENCES categoria(id_categoria),
    FOREIGN KEY (id_fornecedor) REFERENCES fornecedor(id_fornecedor)
);

CREATE TABLE compra (
    id_compra INT IDENTITY(1,1) PRIMARY KEY,
    id_fornecedor INT NOT NULL,
    data_compra DATETIME DEFAULT GETDATE(),
    valor_total DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (id_fornecedor) REFERENCES fornecedor(id_fornecedor)
);

CREATE TABLE item_compra (
    id_item_compra INT IDENTITY(1,1) PRIMARY KEY,
    id_compra INT NOT NULL,
    id_produto INT NOT NULL,
    quantidade INT NOT NULL,
    valor_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (id_compra) REFERENCES compra(id_compra),
    FOREIGN KEY (id_produto) REFERENCES produto(id_produto)
);

CREATE TABLE venda (
    id_venda INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    data_venda DATETIME DEFAULT GETDATE(),
    valor_total DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

CREATE TABLE item_venda (
    id_item_venda INT IDENTITY(1,1) PRIMARY KEY,
    id_venda INT NOT NULL,
    id_produto INT NOT NULL,
    quantidade INT NOT NULL,
    valor_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (id_venda) REFERENCES venda(id_venda),
    FOREIGN KEY (id_produto) REFERENCES produto(id_produto)
);

CREATE TABLE movimentacao_estoque (
    id_movimentacao INT IDENTITY(1,1) PRIMARY KEY,
    id_produto INT NOT NULL,
    id_usuario INT NOT NULL,
    tipo VARCHAR(10) NOT NULL CHECK (tipo IN ('ENTRADA','SAIDA','AJUSTE')),
    quantidade INT NOT NULL,
    data_movimentacao DATETIME DEFAULT GETDATE(),
    observacao VARCHAR(255),
    FOREIGN KEY (id_produto) REFERENCES produto(id_produto),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);
