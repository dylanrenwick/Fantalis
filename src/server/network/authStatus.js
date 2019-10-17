module.exports = class authStatus {
    constructor(address) {
        this.address = address;

        this.versionVerified = false;
        this.loggedIn = false;

        this.accountID = null;
    }

    isAuthenticated() {
        return this.versionVerified && this.loggedIn && this.accountID >= 0;
    }
}