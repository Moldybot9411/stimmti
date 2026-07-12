<script lang="ts">
    import { ChevronLeft } from '@lucide/svelte';
</script>

<button class="btn mt-4 mb-4 ml-4 btn-lg" aria-label="Navigate Back" onclick={() => history.back()}>
    <ChevronLeft />
</button>

<h1 class="mx-auto mb-6 w-fit text-3xl font-bold">Privacy Policy</h1>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
    <div class="join join-vertical h-fit w-full rounded-box bg-base-100">
        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">1. Responsible Entity</div>
            <div class="collapse-content text-sm">
                <p>
                    This project is developed as part of a school assignment and is not intended for commercial use. The responsible entity for data processing is:
                    <br />
                    <br />
                    <strong>NAME</strong>
                    <br />
                    ADRESSE
                    <br />
                    Mail
                </p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">2. Data Collection and Usage</div>
            <div class="collapse-content text-sm">
                <p>
                    We do not collect personal data unless you explicitly provide it (e.g., through a contact form). Any data you provide will only be used for the purposes stated at the time of collection. For example:
                </p>
                <ul class="list">
                    <li class="list-row">- To respond to inquiries</li>
                    <li class="list-row">- To improve the functionality of this project</li>
                </ul>
                <p>No data will be shared with third parties unless required by law.</p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">3. Cookies and Tracking</div>
            <div class="collapse-content text-sm">
                <p>
                    This website does not use cookies, tracking tools, or analytics services that collect personal data. All interactions with this website remain anonymous.
                </p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">4. Data Security</div>
            <div class="collapse-content text-sm">
                <p>
                    We take appropriate technical and organizational measures to protect your data from unauthorized access, loss, or misuse. However, as this is a school project, we recommend not sharing sensitive personal information through this website.
                </p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">5. Your Rights</div>
            <div class="collapse-content text-sm">
                <p>
                    Under the GDPR, you have the following rights regarding your personal data:
                </p>
                <ul class="list">
                    <li class="list-row">- The right to access your data</li>
                    <li class="list-row">- The right to correct inaccurate data</li>
                    <li class="list-row">- The right to request deletion of your data</li>
                    <li class="list-row">- The right to restrict processing of your data</li>
                    <li class="list-row">- The right to data portability</li>
                    <li class="list-row">- The right to object to data processing</li>
                </ul>
                <p>
                    To exercise these rights, please contact us at the email address provided above.
                </p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">6. Contact</div>
            <div class="collapse-content text-sm">
                <p>
                    If you have any questions about this Privacy Policy or how your data is handled, please contact us at:
                    <br />
                    <br />
                    <strong>- MAIL</strong>
                </p>
            </div>
        </div>

        <div class="collapse-arrow collapse join-item border border-base-300">
            <input type="checkbox" />
            <div class="collapse-title font-semibold">7. Disclaimer</div>
            <div class="collapse-content text-sm">
                <p>
                    This project is for educational purposes only and is not intended for public or commercial use. While we strive to comply with all applicable data protection laws, this website is part of a learning exercise and may not meet the standards of a professional website.
                </p>
            </div>
        </div>
    </div>
</div>