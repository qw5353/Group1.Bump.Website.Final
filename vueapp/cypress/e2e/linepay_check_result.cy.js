describe("Check ECPay", () => {
    it("查詢結果", () => {
        cy.viewport(1280, 720);
        cy.visit("https://pay.line.me/portal/tw/auth/login");

        // test_202307281482
        cy.get('.input_wrap').type('test_202307281482')
        cy.get('#password').type('test_202307281482')

        cy.get('#loginActionButton').click()

        cy.wait(2000);

        cy.visit('https://sandbox-pay.line.me/zh_TW/deal/integrate')
        cy.get('[href="#"] > b').click()
    });
});
