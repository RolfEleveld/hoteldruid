window.hotelDruidCulture = {
    getBrowserLanguages: function () {
        if (window.navigator && Array.isArray(window.navigator.languages) && window.navigator.languages.length > 0) {
            return window.navigator.languages;
        }

        if (window.navigator && typeof window.navigator.language === "string" && window.navigator.language.length > 0) {
            return [window.navigator.language];
        }

        return ["en"];
    },

    getStoredCulture: function (storageKey) {
        try {
            return window.localStorage.getItem(storageKey);
        }
        catch {
            return null;
        }
    },

    setStoredCulture: function (storageKey, cultureName) {
        try {
            window.localStorage.setItem(storageKey, cultureName);
        }
        catch {
            // Ignore unavailable storage.
        }
    },

    setDocumentLanguage: function (cultureName) {
        if (!cultureName) {
            return;
        }

        document.documentElement.lang = cultureName;
    }
};