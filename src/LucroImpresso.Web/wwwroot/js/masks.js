window.masks = {
    // Formata valor monetário sem o prefixo "R$" (o Input Group Bootstrap cuida do label visual)
    money: function (input) {
        let value = input.value.replace(/\D/g, "");
        if (value === "") {
            input.value = "";
            return;
        }
        value = (parseInt(value) / 100).toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        input.value = value;
        input.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Máscara de telefone com suporte a celular (9 dígitos) e fixo (8 dígitos)
    phone: function (e) {
        const el = e.target || e;
        const isDeleting = e.inputType === 'deleteContentBackward' || e.inputType === 'deleteContentForward';
        if (isDeleting) return;

        let v = el.value.replace(/\D/g, "").substring(0, 11);
        if (v.length === 0) {
            el.value = "";
            return;
        }

        if (v.length <= 2) {
            el.value = "(" + v;
        } else if (v.length <= 6) {
            el.value = "(" + v.substring(0, 2) + ") " + v.substring(2);
        } else if (v.length <= 10) {
            el.value = "(" + v.substring(0, 2) + ") " + v.substring(2, 6) + "-" + v.substring(6);
        } else {
            el.value = "(" + v.substring(0, 2) + ") " + v.substring(2, 7) + "-" + v.substring(7);
        }
        el.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Alias para celular
    celular: function (e) {
        window.masks.phone(e);
    },

    // Formata peso em gramas com 2 casas decimais (sem " g" — o Input Group cuida disso)
    weight: function (input) {
        let value = input.value.replace(/\D/g, "");
        if (value === "") {
            input.value = "";
            return;
        }
        input.value = parseInt(value).toLocaleString('pt-BR');
        input.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Formata peso com 2 casas decimais sem a string " g"
    weightUnit: function (input) {
        let value = input.value.replace(/\D/g, "");
        if (value === "") {
            input.value = "";
            return;
        }
        value = (parseInt(value) / 100).toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        input.value = value;
        input.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Formata percentual sem a string " %" (o Input Group cuida do label visual)
    percentage: function (input) {
        let value = input.value.replace(/\D/g, "");
        if (value === "") {
            input.value = "";
            return;
        }
        value = (parseInt(value) / 100).toLocaleString('pt-BR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        input.value = value;
        input.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Máscara de tempo HH:MM com tolerância a deleção (recebe o evento)
    time: function (event) {
        const input = event.target;

        // Se o usuário está apagando, deixa o browser agir naturalmente e notifica
        const isDeleting = event.inputType === 'deleteContentBackward'
                        || event.inputType === 'deleteContentForward';
        if (isDeleting) {
            input.dispatchEvent(new Event('change', { bubbles: true }));
            return;
        }

        let value = input.value.replace(/\D/g, "");
        if (value.length > 4) value = value.substring(0, 4);
        if (value === "") {
            input.value = "";
            input.dispatchEvent(new Event('change', { bubbles: true }));
            return;
        }
        if (value.length > 2) {
            input.value = value.substring(0, 2) + ":" + value.substring(2);
        } else {
            input.value = value;
        }
        input.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Lê o valor de um elemento passado via ElementReference do Blazor
    getInputValue: function (element) {
        return element ? element.value : "";
    },

    // Define o valor de um elemento passado via ElementReference do Blazor
    setInputValue: function (element, value) {
        if (element) {
            element.value = value;
        }
    },

    // Máscara de CPF: 000.000.000-00
    cpf: function (e) {
        const el = e.target || e;
        const isDeleting = e.inputType === 'deleteContentBackward' || e.inputType === 'deleteContentForward';
        if (isDeleting) return;

        let v = el.value.replace(/\D/g, "").substring(0, 11);
        if (v.length === 0) {
            el.value = "";
        } else if (v.length <= 3) {
            el.value = v;
        } else if (v.length <= 6) {
            el.value = v.substring(0, 3) + "." + v.substring(3);
        } else if (v.length <= 9) {
            el.value = v.substring(0, 3) + "." + v.substring(3, 6) + "." + v.substring(6);
        } else {
            el.value = v.substring(0, 3) + "." + v.substring(3, 6) + "." + v.substring(6, 9) + "-" + v.substring(9);
        }
        el.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Máscara de CNPJ: 00.000.000/0000-00
    cnpj: function (e) {
        const el = e.target || e;
        const isDeleting = e.inputType === 'deleteContentBackward' || e.inputType === 'deleteContentForward';
        if (isDeleting) return;

        let v = el.value.replace(/\D/g, "").substring(0, 14);
        if (v.length === 0) {
            el.value = "";
        } else if (v.length <= 2) {
            el.value = v;
        } else if (v.length <= 5) {
            el.value = v.substring(0, 2) + "." + v.substring(2);
        } else if (v.length <= 8) {
            el.value = v.substring(0, 2) + "." + v.substring(2, 5) + "." + v.substring(5);
        } else if (v.length <= 12) {
            el.value = v.substring(0, 2) + "." + v.substring(2, 5) + "." + v.substring(5, 8) + "/" + v.substring(8);
        } else {
            el.value = v.substring(0, 2) + "." + v.substring(2, 5) + "." + v.substring(5, 8) + "/" + v.substring(8, 12) + "-" + v.substring(12);
        }
        el.dispatchEvent(new Event('change', { bubbles: true }));
    },

    // Máscara de CEP: 00000-000
    cep: function (e) {
        const el = e.target || e;
        const isDeleting = e.inputType === 'deleteContentBackward' || e.inputType === 'deleteContentForward';
        if (isDeleting) return;

        let v = el.value.replace(/\D/g, "").substring(0, 8);
        if (v.length === 0) {
            el.value = "";
        } else if (v.length <= 5) {
            el.value = v;
        } else {
            el.value = v.substring(0, 5) + "-" + v.substring(5);
        }
        el.dispatchEvent(new Event('change', { bubbles: true }));
    }
};

