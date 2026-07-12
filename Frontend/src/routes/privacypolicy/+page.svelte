<script lang="ts">
    import { ChevronLeft, ShieldAlert, Languages } from '@lucide/svelte';

    let lang = $state<'en' | 'de'>('en');

    function toggleLang() {
        lang = lang === 'en' ? 'de' : 'en';
    }

    const content = {
        en: {
            title: 'Privacy Policy',
            sections: [
                {
                    title: '1. Responsible Entity',
                    body: `<p>This project is developed as part of a school assignment and is not intended for commercial use. The responsible entity for data processing is:<br><br><strong>Max Mustermann</strong><br>Musterstraße 12, 12345 Musterstadt<br>max.mustermann@musterschule-beispiel.de</p>`
                },
                {
                    title: '2. Data Collection and Usage',
                    body: `<p>We do not collect personal data unless you explicitly provide it (e.g., through a survey or contact form). Any data you provide will only be used for the purposes stated at the time of collection. For example:</p>
                    <ul class="list"><li class="list-row">- To display and evaluate survey results</li><li class="list-row">- To respond to inquiries</li><li class="list-row">- To improve the functionality of this project</li></ul>
                    <p>No data will be shared with third parties unless required by law.</p>`
                },
                {
                    title: '3. Cookies and Tracking',
                    body: `<p>This website does not use tracking tools or analytics services that collect personal data. Only technically necessary cookies (e.g., to keep track of your survey session) may be used.</p>`
                },
                {
                    title: '4. Data Security',
                    body: `<p>We take appropriate technical and organizational measures to protect your data from unauthorized access, loss, or misuse. However, as this is a school project, we recommend not sharing sensitive personal information through this website.</p>`
                },
                {
                    title: '5. Your Rights',
                    body: `<p>Under the GDPR, you have the following rights regarding your personal data:</p>
                    <ul class="list"><li class="list-row">- The right to access your data</li><li class="list-row">- The right to correct inaccurate data</li><li class="list-row">- The right to request deletion of your data</li><li class="list-row">- The right to restrict processing of your data</li><li class="list-row">- The right to data portability</li><li class="list-row">- The right to object to data processing</li></ul>
                    <p>To exercise these rights, please contact us at the email address provided above.</p>`
                },
                {
                    title: '6. Contact',
                    body: `<p>If you have any questions about this Privacy Policy or how your data is handled, please contact us at:<br><br><strong>max.mustermann@musterschule-beispiel.de</strong></p>`
                },
                {
                    title: '7. Disclaimer',
                    body: `<p>This project is for educational purposes only and is not intended for public or commercial use. While we strive to comply with all applicable data protection laws, this website is part of a learning exercise and may not meet the standards of a professional website.</p>
                    <p><em>Note: The name, address, and email shown above are placeholder (dummy) data for this school project.</em></p>`
                }
            ],
            notice: 'The name, address, and email shown above are placeholder (dummy) data and must be replaced with real information before any public deployment.'
        },
        de: {
            title: 'Datenschutzerklärung',
            sections: [
                {
                    title: '1. Verantwortliche Stelle',
                    body: `<p>Dieses Projekt wurde im Rahmen eines Schulprojekts entwickelt und ist nicht für die kommerzielle Nutzung bestimmt. Verantwortlich für die Datenverarbeitung ist:<br><br><strong>Max Mustermann</strong><br>Musterstraße 12, 12345 Musterstadt<br>max.mustermann@musterschule-beispiel.de</p>`
                },
                {
                    title: '2. Datenerhebung und -nutzung',
                    body: `<p>Wir erheben keine personenbezogenen Daten, es sei denn, du gibst diese ausdrücklich an (z. B. über eine Umfrage oder ein Kontaktformular). Alle von dir bereitgestellten Daten werden ausschließlich für die zum Zeitpunkt der Erhebung genannten Zwecke verwendet, zum Beispiel:</p>
                    <ul class="list"><li class="list-row">- Zur Anzeige und Auswertung von Umfrageergebnissen</li><li class="list-row">- Zur Beantwortung von Anfragen</li><li class="list-row">- Zur Verbesserung der Funktionalität dieses Projekts</li></ul>
                    <p>Es findet keine Weitergabe an Dritte statt, sofern dies nicht gesetzlich vorgeschrieben ist.</p>`
                },
                {
                    title: '3. Cookies und Tracking',
                    body: `<p>Diese Website verwendet keine Tracking-Tools oder Analyse-Dienste, die personenbezogene Daten erfassen. Es können lediglich technisch notwendige Cookies eingesetzt werden (z. B. um deine Umfrage-Sitzung zu speichern).</p>`
                },
                {
                    title: '4. Datensicherheit',
                    body: `<p>Wir treffen angemessene technische und organisatorische Maßnahmen, um deine Daten vor unbefugtem Zugriff, Verlust oder Missbrauch zu schützen. Da es sich um ein Schulprojekt handelt, empfehlen wir dennoch, keine sensiblen personenbezogenen Informationen über diese Website zu teilen.</p>`
                },
                {
                    title: '5. Deine Rechte',
                    body: `<p>Gemäß DSGVO hast du folgende Rechte bezüglich deiner personenbezogenen Daten:</p>
                    <ul class="list"><li class="list-row">- Recht auf Auskunft</li><li class="list-row">- Recht auf Berichtigung unrichtiger Daten</li><li class="list-row">- Recht auf Löschung deiner Daten</li><li class="list-row">- Recht auf Einschränkung der Verarbeitung</li><li class="list-row">- Recht auf Datenübertragbarkeit</li><li class="list-row">- Recht auf Widerspruch gegen die Verarbeitung</li></ul>
                    <p>Um diese Rechte auszuüben, kontaktiere uns bitte über die oben genannte E-Mail-Adresse.</p>`
                },
                {
                    title: '6. Kontakt',
                    body: `<p>Bei Fragen zu dieser Datenschutzerklärung oder zum Umgang mit deinen Daten kontaktiere uns bitte unter:<br><br><strong>max.mustermann@musterschule-beispiel.de</strong></p>`
                },
                {
                    title: '7. Haftungsausschluss',
                    body: `<p>Dieses Projekt dient ausschließlich Bildungszwecken und ist nicht für die öffentliche oder kommerzielle Nutzung bestimmt. Wir bemühen uns, alle geltenden Datenschutzgesetze einzuhalten, dennoch ist diese Website Teil einer Lernübung und erfüllt möglicherweise nicht die Standards einer professionellen Website.</p>
                    <p><em>Hinweis: Name, Adresse und E-Mail oben sind Platzhalter (Dummy-Daten) für dieses Schulprojekt.</em></p>`
                }
            ],
            notice: 'Name, Adresse und E-Mail oben sind Platzhalter (Dummy-Daten) und müssen vor einer echten Veröffentlichung durch reale Angaben ersetzt werden.'
        }
    };
</script>

<div class="mx-auto flex w-full items-center justify-between md:w-200">
    <button class="btn mt-4 mb-4 ml-4 btn-lg" aria-label="Navigate Back" onclick={() => history.back()}>
        <ChevronLeft />
    </button>

    <button class="btn mt-4 mb-4 mr-4 btn-sm gap-2" aria-label="Toggle language" onclick={toggleLang}>
        <Languages size={16} />
        {lang === 'en' ? 'Deutsch' : 'English'}
    </button>
</div>

<h1 class="mx-auto mb-6 w-fit text-3xl font-bold">{content[lang].title}</h1>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
    <div class="join join-vertical h-fit w-full rounded-box bg-base-100">
        {#each content[lang].sections as section}
            <div class="collapse-arrow collapse join-item border border-base-300">
                <input type="checkbox" />
                <div class="collapse-title font-semibold">{section.title}</div>
                <div class="collapse-content text-sm">
                    {@html section.body}
                </div>
            </div>
        {/each}
    </div>
</div>

<div class="alert alert-warning">
	<ShieldAlert size={20} />
	<span class="text-sm">{content[lang].notice}</span>
</div>